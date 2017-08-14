using UnityEditor;

public class Bundler
{
	const string dir = "AssetBundles";
	const string extension = "";

    [MenuItem("One Window/Build Bundle")]
    static void BuildAllAssetBundles()
    {		
		BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildTarget.StandaloneWindows);
	}


}
