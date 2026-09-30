using UnityEngine;

// Compatibilidade: Unity 6 renomeou velocity para linearVelocity.
public static class RigidbodyExt
{
    public static void SetVelocity(this Rigidbody2D rb, Vector2 v)
    {
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = v;
#else
        rb.velocity = v;
#endif
    }
}
