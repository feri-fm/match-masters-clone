using System;
using Core;
using UnityEngine;

namespace Match3
{
    public class AnimatorTraitView : TraitView<AnimatorTrait>
    {
        public float drag = 0.9f;
        public float squashDistance = 0.1f;
        public AnimationCurve forceOverDistance;
        public Transform body;
        public Transform anchor;
        public Animator animator;
        public TextHelper text;

        private TileView tile;
        private Transform root;

        private Vector2 velocity;

        public override Trait CreateTrait() => new AnimatorTrait();

        protected override void OnSetup()
        {
            base.OnSetup();
            tile = entity as TileView;

            root = tile.root;
            root.parent = anchor;
            root.localPosition = Vector3.zero;
            root.localScale = Vector3.one;

            body.parent = tile.transform.parent;

            entity.entity.onRemoved += () =>
            {
                root.parent = tile.transform;
                root.localPosition = Vector3.zero;
                root.localScale = Vector3.one;
                body.parent = transform;
            };

            trait.onPlayAnimation += (key) =>
            {
                if (key == "SpawnAtTop")
                    body.position = tile.engine.GetPosition(tile.tile.position + Int2.up);
                if (key == "Spawn")
                    body.position = tile.engine.GetPosition(tile.tile.position);

                animator.ResetTrigger("Stretch");
                animator.ResetTrigger("Squash");
                animator.SetTrigger(key);

                text.text = key;
            };
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            var targetPosition = tile.engine.GetPosition(tile.tile.position);
            Vector2 delta = targetPosition - body.position;
            var force = delta.normalized * forceOverDistance.Evaluate(delta.magnitude);

            velocity += force * Time.fixedDeltaTime;
            velocity *= drag;

            body.position += (Vector3)velocity * Time.fixedDeltaTime;

            animator.SetBool("CanSquash", delta.y >= squashDistance);
        }
        protected override void Update()
        {
            base.Update();

            // body.position = Vector3.MoveTowards(body.position, tile.engine.GetPosition(tile.tile.position), speed * Time.deltaTime);
        }
    }

    public class AnimatorTrait : Trait<AnimatorTraitView>
    {
        public event Action<string> onPlayAnimation = delegate { };

        public void SpawnAtTop() => onPlayAnimation.Invoke("SpawnAtTop");
        public void Spawn() => onPlayAnimation.Invoke("Spawn");
        public void Stretch() => onPlayAnimation.Invoke("Stretch");
        public void Squash() => onPlayAnimation.Invoke("Squash");
        public void Explode() => onPlayAnimation.Invoke("Explode");
    }
}