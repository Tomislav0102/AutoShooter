using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Procedurals
{
    public class MapGenerator : MonoBehaviour
    {
        public Vector2Int dimension;
        [Range(0f, 1f)] public float wallPercentage;
        int[,] _grid;
        public Transform par;
        public GameObject spritePrefab;
        GameObject[,] _sprites;
        public Color[] colors;
        [Title("debug")]
        public Vector2Int pos;

        void Start()
        {
            Generate();
        }

        void Generate()
        {
            for (int j = 0; j < dimension.y; j++)
            {
                for (int i = 0; i < dimension.x; i++)
                {
                }
            }

            _grid = new int[dimension.x, dimension.y];
            _sprites = new GameObject[dimension.x, dimension.y];
            for (int j = 0; j < dimension.y; j++)
            {
                for (int i = 0; i < dimension.x; i++)
                {
                    _grid[i, j] = Random.value < wallPercentage ? 1 : 0;
                    Vector2 pos = new Vector2(i, j);
                    _sprites[i, j] = Instantiate(spritePrefab, pos, Quaternion.identity, par);
                }
            }
            
            ColorMap();
        }

        void ColorMap()
        {
            for (int j = 0; j < dimension.y; j++)
            {
                for (int i = 0; i < dimension.x; i++)
                {
                    _sprites[i, j].GetComponent<SpriteRenderer>().color = colors[_grid[i, j]];
                }
            }
            
        }
        [Button]
        void Restart()
        {
            int spritesCount = par.childCount;
            for (int i = 0; i < spritesCount; i++)
            {
                Destroy(par.GetChild(i).gameObject);
            }
            Generate();
        }

        [Button]
        void MapCellular()
        {
            for (int j = 0; j < dimension.y; j++)
            {
                for (int i = 0; i < dimension.x; i++)
                {
                    if (_grid[i, j] == 0) //floor
                    {
                        if (GetNeighbourWallsCount(new Vector2Int(i, j)) >= 5)
                        {
                            _grid[i, j] = 1;
                        }
                    }
                    else if (_grid[i, j] == 1) //wall
                    {
                        if (GetNeighbourWallsCount(new Vector2Int(i, j)) < 4)
                        {
                            _grid[i, j] = 0;
                        }
                    }
                }
            }
            
            ColorMap();
        }
        [Button]
        void MapLife()
        {
            for (int j = 0; j < dimension.y; j++)
            {
                for (int i = 0; i < dimension.x; i++)
                {
                    int wallsCount = GetNeighbourWallsCount(new Vector2Int(i, j));
                    if (_grid[i, j] == 0) //floor
                    {
                        if (wallsCount == 3)
                        {
                            _grid[i, j] = 1;
                        }
                    }
                    else if (_grid[i, j] == 1) //wall
                    {
                        if (wallsCount < 2 || wallsCount > 3)
                        {
                            _grid[i, j] = 0;
                        }
                    }
                }
            }

            ColorMap();
        }

        int GetNeighbourWallsCount(Vector2Int position)
        {
            int count = 0;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                { 
                    if (i == 0 && j == 0) continue;
                    int x = position.x + i;
                    if (x < 0 || x >= dimension.x) continue;
                    int y = position.y + j;
                    if (y < 0 || y >= dimension.y) continue;
                    count += _grid[position.x + i, position.y + j];
                }
            }
            return count;
        }

    }
}
