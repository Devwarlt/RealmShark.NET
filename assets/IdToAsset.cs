using NLog;

namespace RotMGStats.RealmShark.NET.assets
{
    /// <summary>
    /// Id to asset class. Used to convert incoming realm IDs to the corresponding asset.
    /// </summary>
    public class IdToAsset
    {
        private readonly string l;
        private readonly int id;
        private readonly string idName;
        private readonly string display;
        private readonly string clazz;
        private readonly string group;
        private readonly string label;
        private int tileDmg;
        private Projectile[] projectiles = null;
        private readonly string texture;
        private Texture[] textures = null;
        private static readonly Dictionary<int, IdToAsset> objectID = new Dictionary<int, IdToAsset>();
        private static readonly Dictionary<int, IdToAsset> tileID = new Dictionary<int, IdToAsset>();
        private static Logger logger = LogManager.GetLogger(nameof(IdToAsset));

        /// <summary>
        /// Constructor for the object resources.
        /// </summary>
        /// <param name="l">Base string before parsing</param>
        /// <param name="id">Id of the resource</param>
        /// <param name="idName">Name of the resource</param>
        /// <param name="display">Display name of the resource</param>
        /// <param name="clazz">Class of the resource</param>
        /// <param name="projectiles">Projectile min,max,armorPiercing</param>
        /// <param name="texture">Texture name and index used to find the image</param>
        /// <param name="label">Label of the resource</param>
        /// <param name="group">Group of the resource</param>
        public IdToAsset(
            string l, int id, string idName, string display, string clazz, Projectile[] projectiles, string texture,
            string label, string group)
        {
            this.l = l;
            this.id = id;
            this.idName = idName;
            this.display = display;
            this.clazz = clazz;
            this.projectiles = projectiles;
            this.texture = texture;
            this.label = label;
            this.group = group;
        }

        /// <summary>
        /// Constructor for the tile resources.
        /// </summary>
        /// <param name="l">Base string before parsing</param>
        /// <param name="id">Id of the resource</param>
        /// <param name="damage">Tile damage</param>
        /// <param name="idName">Name of the resource</param>
        /// <param name="texture">Texture name and index used to find the image</param>
        public IdToAsset(string l, int id, int damage, string idName, string texture)
        {
            this.l = l;
            this.id = id;
            this.tileDmg = damage;
            this.idName = idName;
            this.texture = texture;

            display = "";
            clazz = "";
            group = "";
            label = "";
        }

        static IdToAsset()
        {
            ReadObjectList();
            ReadTileList();
        }

        /// <summary>
        /// Reloads assets from files.
        /// </summary>
        public static void ReloadAssets()
        {
            objectID.Clear();
            tileID.Clear();
            ReadObjectList();
            ReadTileList();
        }

        /// <summary>
        /// Method to grab the full list of object resources from file and construct the dictionary.
        /// </summary>
        private static void ReadObjectList()
        {
            var objectsFile = new FileInfo(AssetExtractor.ASSETS_OBJECT_FILE_DIR_PATH);
            if (!objectsFile.Exists) return;
            string lineCheck = "";
            try
            {
                using (var br = new StreamReader(objectsFile.OpenRead()))
                {
                    string line;
                    while ((line = br.ReadLine()) != null)
                    {
                        var l = line.Split(';');
                        lineCheck = line;
                        int id = int.Parse(l[0]);
                        string display = l[1];
                        string clazz = l[2];
                        string group = l[3];
                        string projectile = l[4];
                        var projectiles = ParseProjectile(projectile);
                        string texture = l[5];
                        string label = l[6];
                        string idName = l[7];
                        objectID[id] = new IdToAsset(line, id, idName, display, clazz, projectiles, texture, label, group);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, lineCheck);
                logger.Log(LogLevel.Error, e);
            }

            objectID[-1] = new IdToAsset("", -1, "Unloaded", "Unloaded", "", null, "", "", "Unloaded");
        }

        /// <summary>
        /// Method to grab the full list of tile resources from file and construct the dictionary.
        /// </summary>
        private static void ReadTileList()
        {
            var tilesFile = new FileInfo(AssetExtractor.ASSETS_TILE_FILE_DIR_PATH);
            if (!tilesFile.Exists) return;
            try
            {
                using (var br = new StreamReader(tilesFile.OpenRead()))
                {
                    string line;
                    while ((line = br.ReadLine()) != null)
                    {
                        var l = line.Split(';');
                        int id = int.Parse(l[0]);
                        string texture = l[1];
                        string dmg = l[2];
                        int damage = 0;
                        if (!string.IsNullOrEmpty(dmg))
                        {
                            var s = dmg.Split(',');
                            if (s[0] == s[1])
                            {
                                damage = int.Parse(s[0]);
                            }
                            else
                            {
                                logger.Log(LogLevel.Warn, "Nonuniform tile damage");
                            }
                        }
                        string idName = l[3];
                        tileID[id] = new IdToAsset(line, id, damage, idName, texture);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, e);
            }

            tileID[-1] = new IdToAsset("", -1, -1, "Unknown", "");
        }

        public static void Main(string[] args)
        {
        }

        /// <summary>
        /// Method to grab the name of the object resource.
        /// If display name is not present, use the regular name.
        /// </summary>
        /// <param name="id">Id of the object.</param>
        /// <returns>Best descriptive name of the resource</returns>
        public static string ObjectName(int id)
        {
            if (!objectID.TryGetValue(id, out var i)) return null;
            return string.IsNullOrEmpty(i.display) ? i.idName : i.display;
        }

        /// <summary>
        /// Method to grab the name of the tile resource.
        /// If display name is not present, use the regular name.
        /// </summary>
        /// <param name="id">Id of the tile.</param>
        /// <returns>Best descriptive name of the resource</returns>
        public static string TileName(int id)
        {
            if (!tileID.TryGetValue(id, out var i)) return null;
            return i.idName;
        }

        /// <summary>
        /// Common name of the object.
        /// </summary>
        /// <param name="id">Id of the object.</param>
        /// <returns>Regular name of the object.</returns>
        public static string GetObjectIdName(int id)
        {
            if (!objectID.TryGetValue(id, out var i)) return null;
            return i.idName;
        }

        /// <summary>
        /// Display name of the object.
        /// </summary>
        /// <param name="id">Id of the object.</param>
        /// <returns>Display name of the object.</returns>
        public static string GetDisplayName(int id)
        {
            if (!objectID.TryGetValue(id, out var i)) return null;
            return i.display;
        }

        /// <summary>
        /// Class of the object.
        /// </summary>
        /// <param name="id">Id of the object.</param>
        /// <returns>Class name of the object.</returns>
        public static string GetClazz(int id)
        {
            if (!objectID.TryGetValue(id, out var i)) return null;
            return i.clazz;
        }

        /// <summary>
        /// Group of the object.
        /// </summary>
        /// <param name="id">Id of the object.</param>
        /// <returns>Group name of the object.</returns>
        public static string GetIdGroup(int id)
        {
            if (!objectID.TryGetValue(id, out var i)) return null;
            return i.group;
        }

        /// <summary>
        /// Label of the object.
        /// </summary>
        /// <param name="id">Id of the object.</param>
        /// <returns>Label of the object.</returns>
        public static string GetIdLabel(int id)
        {
            if (!objectID.TryGetValue(id, out var i)) return null;
            return i.label;
        }

        /// <summary>
        /// Parses the projectile string to the number of projectiles the entity can shoot.
        /// </summary>
        /// <param name="projectile">Projectile string</param>
        /// <returns>List of parsed projectiles</returns>
        private static Projectile[] ParseProjectile(string projectile)
        {
            var l = projectile.Split(',');
            string s = l[0];
            int slotType = string.IsNullOrEmpty(s) ? 0 : int.Parse(s);
            int length = l.Length - 1;
            var p = new Projectile[length / 3];
            int index = 0;
            for (int i = 0; i < length; i += 3)
            {
                int min = int.Parse(l[1 + i]);
                int max = int.Parse(l[2 + i]);
                bool ap = l[3 + i] == "1";
                p[index] = new Projectile(min, max, ap, slotType);
                index++;
            }

            return p;
        }

        /// <summary>
        /// Parses the texture string to the texture object.
        /// </summary>
        /// <param name="entity">Entity that should be texture parsed</param>
        /// <returns>List of parsed textures</returns>
        private static Texture[] ParseObjectTexture(IdToAsset entity)
        {
            var l = entity.texture.Split(',');
            var t = new Texture[l.Length / 2];
            int index = 0;
            try
            {
                for (int i = 0; i < l.Length; i += 2)
                {
                    string name = l[i + 1];
                    int ix = int.Parse(l[i]);
                    t[index] = new Texture(name, ix);
                    index++;
                }
            }
            catch (Exception)
            {
                logger.Log(LogLevel.Error, entity);
            }
            return t;
        }

        /// <summary>
        /// Gets the damage the tile makes when walking on it.
        /// </summary>
        /// <param name="id">Id of the tile</param>
        /// <returns>Damage of the tile when walking on it</returns>
        public static int GetTileDamage(int id)
        {
            if (!tileID.TryGetValue(id, out var i)) return -1;
            return i.tileDmg;
        }

        /// <summary>
        /// Minimum damage of weapon.
        /// </summary>
        /// <param name="id">Id of the object.</param>
        /// <param name="projectileId">Bullet sub id</param>
        /// <returns>Minimum damage</returns>
        public static int GetIdProjectileMinDmg(int id, int projectileId)
        {
            if (!objectID.TryGetValue(id, out var i)) return -1;
            return i.projectiles[projectileId].min;
        }

        /// <summary>
        /// Maximum damage of weapon.
        /// </summary>
        /// <param name="id">Id of the object.</param>
        /// <param name="projectileId">Bullet sub id</param>
        /// <returns>Maximum damage</returns>
        public static int GetIdProjectileMaxDmg(int id, int projectileId)
        {
            if (!objectID.TryGetValue(id, out var i)) return -1;
            return i.projectiles[projectileId].max;
        }

        /// <summary>
        /// Checks if the projectile armor pierces.
        /// </summary>
        /// <param name="id">Id of the object.</param>
        /// <param name="projectileId">Bullet sub id</param>
        /// <returns>True if armor pierces</returns>
        public static bool GetIdProjectileArmorPierces(int id, int projectileId)
        {
            if (!objectID.TryGetValue(id, out var i)) return false;
            return i.projectiles[projectileId].ap;
        }

        /// <summary>
        /// Inventory slot type of weapon.
        /// </summary>
        /// <param name="id">Id of the weapon.</param>
        /// <returns>Inventory slot type of weapon</returns>
        public static int GetIdProjectileSlotType(int id)
        {
            if (!objectID.TryGetValue(id, out var i)) return 0;
            return i.projectiles[0].slotType;
        }

        /// <summary>
        /// Object texture file name.
        /// </summary>
        /// <param name="id">Id of the object.</param>
        /// <param name="num">Sub texture number</param>
        /// <returns>File name of the texture</returns>
        public static string GetObjectTextureName(int id, int num)
        {
            if (!objectID.TryGetValue(id, out var i)) return null;
            if (i.textures == null) i.textures = ParseObjectTexture(i);
            try
            {
                return i.textures[num].name;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Object texture file index.
        /// </summary>
        /// <param name="id">Id of the object.</param>
        /// <param name="num">Sub texture number</param>
        /// <returns>File index of the texture</returns>
        public static int GetObjectTextureIndex(int id, int num)
        {
            if (!objectID.TryGetValue(id, out var i)) return 0;
            if (i.textures == null) i.textures = ParseObjectTexture(i);
            if (i.textures == null) return 0;
            try
            {
                return i.textures[num].index;
            }
            catch (IndexOutOfRangeException)
            {
                logger.Log(LogLevel.Error, $"{id} {i}");
                return 0;
            }
        }

        /// <summary>
        /// Tile texture file name.
        /// </summary>
        /// <param name="id">Id of the object.</param>
        /// <param name="num">Sub texture number</param>
        /// <returns>File name of the texture</returns>
        public static string GetTileTextureName(int id, int num)
        {
            if (!tileID.TryGetValue(id, out var i)) return null;
            if (i.textures == null) i.textures = ParseObjectTexture(i);
            if (i.textures == null) return null;
            return i.textures[num].name;
        }

        /// <summary>
        /// Tile texture file index.
        /// </summary>
        /// <param name="id">Id of the object.</param>
        /// <param name="num">Sub texture number</param>
        /// <returns>File index of the texture</returns>
        public static int GetTileTextureIndex(int id, int num)
        {
            if (!tileID.TryGetValue(id, out var i)) return -1;
            if (i.textures == null) i.textures = ParseObjectTexture(i);
            if (i.textures == null) return 0;
            return i.textures[num].index;
        }

        /// <summary>
        /// Checks if the tile id exists.
        /// </summary>
        /// <param name="id">Id of the tile.</param>
        /// <returns>True if the tile ID exists.</returns>
        public static bool TileIdExists(int id)
        {
            return tileID.ContainsKey(id);
        }

        /// <summary>
        /// Simple class to store projectile info
        /// </summary>
        public class Projectile
        {
            public int min; // min dmg
            public int max; // max dmg
            public bool ap; // armor piercing
            public int slotType; // weapon slot

            public Projectile(int min, int max, bool ap, int slotType)
            {
                this.min = min;
                this.max = max;
                this.ap = ap;
                this.slotType = slotType;
            }
        }

        /// <summary>
        /// Simple class to store texture info
        /// </summary>
        private class Texture
        {
            public string name;
            public int index;

            public Texture(string name, int index)
            {
                this.name = name;
                this.index = index;
            }
        }

        public override string ToString()
        {
            return l;
        }
    }
}