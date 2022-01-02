
//Script name : CustomList.cs
using UnityEngine;
using System;
using System.Collections.Generic; // Import the System.Collections.Generic class to give us access to List<>

#region CM
public enum CinemaestreEffectType { SLIDE, PAN, ZOOM, FADE, DELAY }
	public enum SlideType { WORLD_POS, LOCAL_OFFSET, DIR_AND_MAG }
	public enum PanDirection { HORIZONTAL, VERTICAL }

	#region CINEMAESTRE EFFECTS
	[System.Serializable]
	public class CinemaestreEffect {
		public CinemaestreEffectType effectType;

		public float duration = 1f;

		public bool loop = false;
		public int iterations = 1;
		public bool loopForever = true; 
		public bool pingpong = false; 

		public LeanTweenType easeType;
		public bool customEase = false;
		public AnimationCurve easeAnimationCurve; 

		public SlideType slideType;
		public Vector3 slideWorldPos; 
		public Vector3 slideLocalOffset;
		public Vector3 slideMoveDir; 
		public float slideMoveDistance;

		public PanDirection panDirection;
		public bool panCustomDirection = false;
		public Vector3 panAxisOfRotation; // only show this if custom direction is true
		public bool panGlobalSpace;
		public float panAngle;

		public float zoomTargetFOV;

		public Color fadeColor;
		public bool fadeOut = true;
	}
	#endregion

	[System.Serializable]
	public class CinemaestreStack {
		public CinemaestreEffect[] effects;

		// each stack knows how to update on its own
	}
	#endregion
 
public class CustomList : MonoBehaviour {
 
    //This is our custom class with our variables
    [System.Serializable]
    public class MyClass{
        public GameObject AnGO;
        public int AnInt;
        public float AnFloat;
        public Vector3 AnVector3;
        public int[] AnIntArray = new int[0];
		public CinemaestreEffect[] effects;
    }
 
    //This is our list we want to use to represent our class as an array.
    public List<MyClass> MyList = new List<MyClass>(1);
 
 
    void AddNew(){
        //Add a new index position to the end of our list
        MyList.Add(new MyClass());
    }
 
    void Remove(int index){
        //Remove an index position from our list at a point in our list array
        MyList.RemoveAt(index);
    }
}
 