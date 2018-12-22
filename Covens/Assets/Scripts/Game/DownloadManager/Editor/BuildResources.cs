
using UnityEditor;
using UnityEditor.iOS;    
 

#if ENABLE_IOS_ON_DEMAND_RESOURCES 
public class BuildResources 
{ 
	[InitializeOnLoadMethod] 
	static void SetupResourcesBuild()  
	{
		UnityEditor.iOS.BuildPipeline.collectResources += CollectResources;
	}  
	static UnityEditor.iOS.Resource[] CollectResources()
	{ 
		return new Resource[] {
			new Resource("asset-bundle-name", "path/to/asset-bundle").AddOnDemandResourceTags("asset-bundle-name-tag"), 
		};
	}
}  

#endif