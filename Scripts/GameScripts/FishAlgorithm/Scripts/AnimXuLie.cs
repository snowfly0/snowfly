using UnityEngine;
using System.Collections;

public class AnimXuLie: MonoBehaviour 
{  
 private float timeElasped=0.0f;
 public int curFrame=0;
 private float fps=18.0f;
//	private bool control=true;
 public Texture2D[] ani;	
	
 void Update()
	{
	//	print("curframe:"+curFrame);
	  timeElasped+=Time.deltaTime;
		if(timeElasped>=1.0/fps)
		{
	    	timeElasped=0.0f;
			  curFrame++;
          if(curFrame>=ani.Length)
		   {
			curFrame=0;
		   }
           gameObject.GetComponent<Renderer>().material.mainTexture=ani[curFrame] ;
	  }
	}
}