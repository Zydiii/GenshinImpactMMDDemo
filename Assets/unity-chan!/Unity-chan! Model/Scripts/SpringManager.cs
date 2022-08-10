//
//SpingManager.cs for unity-chan!
//
//Original Script is here:
//ricopin / SpingManager.cs
//Rocket Jump : http://rocketjump.skr.jp/unity3d/109/
//https://twitter.com/ricopin416
//
//Revised by N.Kobayashi 2014/06/24
//           Y.Ebata
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityChan
{
	public class SpringManager : MonoBehaviour
	{
		//Kobayashi
		// DynamicRatio is paramater for activated level of dynamic animation 
		public float dynamicRatio = 1.0f;

		//Ebata
		public float			stiffnessForce;
		public AnimationCurve	stiffnessCurve;
		public float			dragForce;
		public AnimationCurve	dragCurve;
		public SpringBone[] springBones;

		void Start ()
		{
			UpdateParameters ();
		}
	
		void Update ()
		{
#if UNITY_EDITOR
		//Kobayashi
		if(dynamicRatio >= 1.0f)
			dynamicRatio = 1.0f;
		else if(dynamicRatio <= 0.0f)
			dynamicRatio = 0.0f;
		//Ebata
		UpdateParameters();
#endif
		}
	
		private void LateUpdate ()
		{
			//Kobayashi
			if (dynamicRatio != 0.0f) {
				for (int i = 0; i < springBones.Length; i++) {
					if (dynamicRatio > springBones [i].threshold) {
						springBones [i].UpdateSpring ();
					}
				}
			}
		}

		private void UpdateParameters ()
		{
			UpdateParameter ("stiffnessForce", stiffnessForce, stiffnessCurve);
			UpdateParameter ("dragForce", dragForce, dragCurve);
		}
	
		private void UpdateParameter (string fieldName, float baseValue, AnimationCurve curve)
		{
			var start = curve.keys [0].time;
			var end = curve.keys [curve.length - 1].time;
			//var step	= (end - start) / (springBones.Length - 1);
		
			var prop = springBones [0].GetType ().GetField (fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
		
			for (int i = 0; i < springBones.Length; i++) {
				//Kobayashi
				if (!springBones [i].isUseEachBoneForceSettings) {
					var scale = curve.Evaluate (start + (end - start) * i / (springBones.Length - 1));
					prop.SetValue (springBones [i], baseValue * scale);
				}
			}
		}

		[ContextMenu("绑定宵宫骨骼")]
		public void setXiaoGongBones()
        {

			List<SpringCollider> collider = new List<SpringCollider>();
			Transform[] transforms = GetComponentsInChildren<Transform>();
			foreach (Transform transform in transforms)
			{
				if (transform.name.StartsWith("MyCollider_"))
				{
					collider.Add(transform.gameObject.GetComponent<SpringCollider>());
				}
			}
			foreach (Transform transform in transforms)
			{
				if ((transform.name.StartsWith("裙_") 
					|| transform.name.StartsWith("繩") 
					|| transform.name.StartsWith("Connection")
					|| transform.name.StartsWith("側髮_")
					|| transform.name.StartsWith("头饰")
					|| transform.name.StartsWith("髮")) 
					&& transform.childCount > 0)
                {
					if (transform.gameObject.GetComponent<SpringBone>() == null)
                    {
						transform.gameObject.AddComponent<SpringBone>();
						transform.gameObject.GetComponent<SpringBone>().child = transform.GetChild(0);
						transform.gameObject.GetComponent<SpringBone>().boneAxis = new Vector3(0, 1, 0);
					}
					transform.gameObject.GetComponent<SpringBone>().colliders = collider.ToArray();
				}
			}
			springBones = GetComponentsInChildren<SpringBone>();
        }
	}
}