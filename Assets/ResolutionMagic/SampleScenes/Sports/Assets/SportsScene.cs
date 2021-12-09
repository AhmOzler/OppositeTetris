using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResolutionMagic{
public class SportsScene : MonoBehaviour
{

    void Start()
    {

    }
   public void FlipGravity()
   {
       Physics2D.gravity *=-1;

   }


   public void ChangeZoonType()
   {
       if(ResolutionManager.Instance.ZoomTo == ResolutionManager.ZoomType.AlwaysDisplayedArea)
       {
           ResolutionManager.Instance.ZoomTo = ResolutionManager.ZoomType.MaximumBounds;
       }
       else
       {
           ResolutionManager.Instance.ZoomTo = ResolutionManager.ZoomType.AlwaysDisplayedArea;
       }
       ResolutionManager.Instance.RefreshResolution();
   }
}
}
