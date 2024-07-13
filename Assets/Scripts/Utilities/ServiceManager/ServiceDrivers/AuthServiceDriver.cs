using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AuthServiceDriver : ServiceDriver
{
    public virtual void Login(UnityAction<Auth> callback)
    {

    }
}

public class Auth
{
    public string id;
}
