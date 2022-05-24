using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Tiled2Unity
{
    public class TiledMap : MonoBehaviour
    {
        public enum MapOrientation
        {
            Orthogonal,
            Isometric,
            Staggered,
            Hexagonal,
        }

        public enum MapStaggerAxis
        {
            X,
            Y,
        }

        public enum MapStaggerIndex
        {
            Odd,
            Even,
        }

        public MapOrientation Orientation = MapOrientation.Orthogonal;
        public MapStaggerAxis StaggerAxis = MapStaggerAxis.X;
        public MapStaggerIndex StaggerIndex = MapStaggerIndex.Odd;
        public int HexSideLength = 0;

        public int NumLayers = 0;
        public int NumTilesWide = 0;
        public int NumTilesHigh = 0;
        public int TileWidth = 0;
        public int TileHeight = 0;
        public float ExportScale = 1.0f;

        // Note: Because maps can be isometric and staggered we simply can't multiply tile width (or height) by number of tiles wide (or high) to get width (or height)
        // We rely on the exporter to calculate the width and height of the map
        public int MapWidthInPixels = 0;
        public int MapHeightInPixels = 0;

        // Background color could be used to set the camera clear color to get the same effect as in Tiled
        public Color BackgroundColor = Color.black;

        private static TiledMap _instance = null;
        public static TiledMap instance
        {
            get
            {
                if (!_instance)
                {
                    TiledMap found = FindObjectOfType<TiledMap>();
                    // We never set the instance so create a new SoundManager!
                    if (found && found.gameObject.activeSelf)
                    {
                        // Use the one you just found.
                        _instance = found;

                    }
                    else
                    {
                        Debug.LogError("You try to get the TileMap of this scene, but there isn't any!");
                    }
                }

                return _instance;
            }
        }

        public Vector3 tileToUnity(Vector2 tilePos)
        {
            Vector2 unitySpace = ((tilePos * TileWidth) * ExportScale);
            Vector3 bla = new Vector3 (unitySpace.x, -unitySpace.y, gameObject.transform.position.z);
            return new Vector3 (bla.x + ((TileWidth / 2) * ExportScale), 
                bla.y - ((TileWidth / 2) * ExportScale), 
                bla.z);
        }

        public Vector2 unityToTile(Vector3 position)
        {
            Vector3 tilePosVec = (position / ExportScale) / TileWidth;
            return new Vector2 ((int)(tilePosVec).x, -(int)(tilePosVec).y);
        }


        public float GetMapWidthInPixelsScaled()
        {
            return this.MapWidthInPixels * this.transform.lossyScale.x * this.ExportScale;
        }

        public float GetMapHeightInPixelsScaled()
        {
            return this.MapHeightInPixels * this.transform.lossyScale.y * this.ExportScale;
        }

        public Rect GetMapRect()
        {
            Vector2 pos_w = this.gameObject.transform.position;
            float width = this.MapWidthInPixels;
            float height = this.MapHeightInPixels;
            return new Rect(pos_w.x, pos_w.y - height, width, height);
        }

        public Rect GetMapRectInPixelsScaled()
        {
            Vector2 pos_w = this.gameObject.transform.position;
            float widthInPixels = GetMapWidthInPixelsScaled();
            float heightInPixels = GetMapHeightInPixelsScaled();
            return new Rect(pos_w.x, pos_w.y - heightInPixels, widthInPixels, heightInPixels);
        }

        public bool AreTilesStaggered()
        {
            // Hex and Iso Staggered maps both use "staggered" tiles
            return this.Orientation == MapOrientation.Staggered || this.Orientation == MapOrientation.Hexagonal;
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 pos_w = this.gameObject.transform.position;
            Vector3 topLeft = Vector3.zero + pos_w;
            Vector3 topRight = new Vector3(GetMapWidthInPixelsScaled(), 0) + pos_w;
            Vector3 bottomRight = new Vector3(GetMapWidthInPixelsScaled(), -GetMapHeightInPixelsScaled()) + pos_w;
            Vector3 bottomLeft = new Vector3(0, -GetMapHeightInPixelsScaled()) + pos_w;

            // To make gizmo visible, even when using depth-shader shaders, we decrease the z depth by the number of layers
            float depth_z = -1.0f * this.NumLayers;
            pos_w.z += depth_z;
            topLeft.z += depth_z;
            topRight.z += depth_z;
            bottomRight.z += depth_z;
            bottomLeft.z += depth_z;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }
    }
}
