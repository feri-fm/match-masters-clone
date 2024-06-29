using System;
using MMC.EngineCore;
using UnityEngine;

namespace MMC.Match3
{
    public class AnimatorTraitView : TraitView<AnimatorTrait>
    {
        public float drag = 0.9f;
        public float squashDistance = 0.1f;
        public float speed = 20;
        public AnimationCurve forceOverDistance;
        public Transform body;
        public Transform anchor;
        public Animator animator;
        public TextAdaptor text;

        private TileView tile;
        private Transform root;

        private Vector2 velocity;

        private bool isMovingTo;
        private string lastAnimationKey;

        public override Trait CreateTrait() => new AnimatorTrait();

        protected override void OnSetup()
        {
            base.OnSetup();
            tile = entity as TileView;

            root = tile.root;
            root.parent = anchor;
            root.localPosition = Vector3.zero;
            root.localRotation = Quaternion.identity;
            root.localScale = Vector3.one;

            body.parent = tile.transform.parent;

            entity.entity.onRemoved += () =>
            {
                root.parent = tile.transform;
                root.localPosition = Vector3.zero;
                root.localRotation = Quaternion.identity;
                root.localScale = Vector3.one;
                body.parent = transform;
            };

            trait.onPlayAnimation += (key) =>
            {
                lastAnimationKey = key;
                if (key == "SpawnAtTop")
                    body.position = tile.engine.GetPosition(tile.tile.position + Int2.up * 2);
                if (key == "Spawn")
                    body.position = tile.engine.GetPosition(tile.tile.position);
                if (key == "Jump")
                {
                    body.position = tile.engine.GetPosition(tile.tile.position);
                    velocity = Vector2.zero;
                }
                if (key == "Stop")
                    velocity = Vector2.zero;

                animator.ResetTrigger("Stretch");
                animator.ResetTrigger("Squash");
                animator.SetTrigger(key);

                text.text = key;
            };

            trait.onAddForce += (f) =>
            {
                velocity += f;
            };

            trait.onMoveTo += () =>
            {
                isMovingTo = true;
            };
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isMovingTo)
            {
                var targetPosition = tile.engine.GetPosition(tile.tile.position);
                Vector2 delta = targetPosition - body.position;
                var force = delta.normalized * forceOverDistance.Evaluate(delta.magnitude);

                velocity += force * Time.fixedDeltaTime;
                velocity *= drag;

                body.position += (Vector3)velocity * Time.fixedDeltaTime;

                animator.SetBool("CanSquash", delta.y >= squashDistance);

                if (lastAnimationKey == "Squash" && delta.y >= squashDistance)
                {
                    lastAnimationKey = "";
                    velocity = Vector2.zero;
                }
            }
        }
        protected override void Update()
        {
            base.Update();
            if (isMovingTo)
            {
                velocity = Vector2.zero;
                var targetPosition = tile.engine.GetPosition(tile.tile.position);
                body.position = Vector3.MoveTowards(body.position, targetPosition, speed * Time.deltaTime);
                if (body.position == targetPosition)
                {
                    isMovingTo = false;
                }
            }
        }
    }

    public class AnimatorTrait : Trait<AnimatorTraitView>
    {
        public event Action<string> onPlayAnimation = delegate { };
        public event Action<Vector2> onAddForce = delegate { };
        public event Action onMoveTo = delegate { };

        public void SpawnAtTop() => onPlayAnimation.Invoke("SpawnAtTop");
        public void Spawn() => onPlayAnimation.Invoke("Spawn");
        public void Stretch() => onPlayAnimation.Invoke("Stretch");
        public void Squash() => onPlayAnimation.Invoke("Squash");
        public void Jump() => onPlayAnimation.Invoke("Jump");
        public void MoveTo() => onMoveTo.Invoke();
        public void AddForce(Vector2 force) => onAddForce.Invoke(force);
    }
}