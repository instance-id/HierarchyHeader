using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
	public Dummy RefToDummy;
	[SerializeField] public Dummy refToDummy2;
	[SerializeField] private float number;
	[SerializeField] private GameObject someGameObject;
}