using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FollowCanvas : MonoBehaviour
{
   [SerializeField] private Transform _cartToFollow;

    public Transform CartToFollow
    {
        get => _cartToFollow;
        set => _cartToFollow = value;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(_cartToFollow.position.x - 0.9f, transform.position.y, _cartToFollow.position.z + .5f), 3f);
    }
}