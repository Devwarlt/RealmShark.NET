using System.Resources;

namespace RotMGStats.RealmShark.NET.assets
{
    /// <summary>
    /// Main loader for assets. If assets are missing or are outdated, extracts the assets from realm resources files.
    /// </summary>
    public class AssetExtractor
    {
        public static readonly string ASSETS_OBJECT_FILE_DIR_PATH = "assets/ObjectID.list";
        public static readonly string ASSETS_TILE_FILE_DIR_PATH = "assets/TileID.list";
    }
}