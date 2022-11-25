using System.Collections.Generic;

using UnityEngine;

using InteligenciaArtificial.SegundoParcial.Utils;
using InteligenciaArtificial.SegundoParcial.Utils.CameraHandler;

namespace InteligenciaArtificial.SegundoParcial.Handlers.Map
{
    [System.Serializable]
    public class Cell
    {
        #region PRIVATE_FIELDS
        private Vector2Int position;
        private Vector2Int size;
        private List<Vector2Int> adjacents;
        #endregion

        #region PORPERTIES
        public Vector2Int Size => size;
        public Vector2Int Position => position;
        public List<Vector2Int> Adjacents => adjacents;
        #endregion

        #region CONSTRUCTOR
        public Cell(Vector2Int position, Vector2Int size)
        {
            this.position = position;
            this.size = size;
            adjacents = new List<Vector2Int>();
        }
        #endregion

        #region PUBLIC_METHODS
        public void SetAdjacentsCells(Vector2Int[] adjacents)
        {
            this.adjacents.AddRange(adjacents);
        }
        #endregion
    }

    public class MapHandler : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField, Tooltip("The max size of the grid on X coordinate."),Range(100,300)] private int maxGridX = 100;
        [SerializeField, Tooltip("The max size of the grid on Y coordinate."),Range(100,300)] private int maxGridY = 100;
        [SerializeField, Tooltip("The max size of each cell that is on the map.")] private Vector2Int sizeCells = default;
        [SerializeField] private float offsetPerCell = 0;
        [SerializeField] private GameObject prefabCellView = null;
        [SerializeField] private Transform holder = null;
        #endregion

        #region PRIVATE_FIELDS
        private Cell middleCell = null;
        private Camera mainCamera = null;
        private Dictionary<Vector2Int, Cell> map = null;
        #endregion

        #region PROPERTIES
        public int MaxGridX { get { return maxGridX; } }
        public Dictionary<Vector2Int, Cell> Map => map;
        #endregion

        #region PUBLIC_METHODS
        public void Init()
        {
            mainCamera = Camera.main;

            map = new Dictionary<Vector2Int, Cell>();

            GenerateGrid();
        }

        public Cell GetCellByPosition(Vector2Int cellPosition)
        {
            if (!map.ContainsKey(cellPosition))
            {
                Debug.LogWarning("WARNING: Invalid cell position. Check the parameter of GetCellByPosition()");
                return null;
            }

            return map[cellPosition];
        }

        public List<Vector2Int> GetRandomUniquePositions(int initialPopulationSize)
        {
            List<Vector2Int> result = new List<Vector2Int>();

            if (initialPopulationSize <= (maxGridX * 2)) //Por dos ya que al ser dos equipos cada uno puede ocupar 100 celdas de arriba para abajo.
            {
                for (int i = 0; i < initialPopulationSize; i++)
                {
                    Vector2Int newPosition = SimulationConstants.InvalidPosition; 
                    do
                    {

                        newPosition = GetRanodmPosition();

                    } while (result.Contains(newPosition));

                    result.Add(newPosition);
                }
            }
            else
            {
                Debug.Log("The population is exeding the grid size of the map.");
                return null;
            }

            return result;
        }

        public Vector2Int GetRanodmPosition()
        {
            return map[new Vector2Int(Random.Range(0, maxGridX), Random.Range(0, maxGridY))].Position;
        }

        public List<Cell> GetLeftToRightBottomCells()
        {
            List<Cell> listOfCells = new List<Cell>();

            foreach (Cell cell in map.Values)
            {
                if(cell.Position != SimulationConstants.InvalidPosition)
                {
                    if(cell.Position.y < 1)
                    {
                        listOfCells.Add(cell);
                    }
                }
            }

            //No nesecito reorganizar la lista ya que viene ordenada por como arme la grid
            return listOfCells;
        }

        public List<Cell> GetRightToLeftTopCells()
        {
            List<Cell> listOfCells = new List<Cell>();

            foreach (Vector2Int gridPos in map.Keys)
            {
                if (gridPos != SimulationConstants.InvalidPosition)
                { 
                    if (gridPos.y >= maxGridY-1)
                    {
                        listOfCells.Add(map[gridPos]);
                    }
                }
            }

            //Reorganizo la lista para que me quede invertida si es arriba de derecha a izquierda
            List<Cell> sortedList = new List<Cell>();
            for (int i = listOfCells.Count-1; i > 0 ; i--)
            {
                sortedList.Add(listOfCells[i]);
            }
            return sortedList;
        }
        #endregion

        #region PRIVATE_METHODS
        private void GenerateGrid()
        {
            for (int x = 0; x < maxGridX; x++)
            {
                for (int y = 0; y < maxGridY; y++)
                {
                    Vector2Int cellPosition = new Vector2Int((int)(x * (sizeCells.x + offsetPerCell)),(int)(y * (sizeCells.y + offsetPerCell)));
                    Vector2Int gridPosition = new Vector2Int(x,y);

                    Cell gridCell = new Cell(cellPosition, sizeCells);

                    if(!map.ContainsKey(cellPosition))
                    {
                        map.Add(gridPosition, gridCell);
                    }

                    Instantiate(prefabCellView, new Vector2(cellPosition.x, cellPosition.y), Quaternion.identity, holder);
                }
            }

            ConfigureCamera();
        }

        private Vector2Int GetMiddleOfMap()
        {
            foreach (Cell cell in map.Values)
            {
                if(cell.Position.x == (int)(((maxGridX + maxGridY) * 0.5f)) && 
                    cell.Position.y == (int)(((maxGridY + maxGridX) * 0.5f)))
                {
                    middleCell = cell;
                    return cell.Position;
                }
            }

            return new Vector2Int(-1,-1);
        }

        private void ConfigureCamera()
        {
            Vector2Int middleMap = GetMiddleOfMap();
            Vector2 finalCameraPosition = new Vector2(middleMap.x + sizeCells.x, middleMap.y + sizeCells.y);

            mainCamera.transform.position = new Vector3(finalCameraPosition.x, finalCameraPosition.y, mainCamera.transform.position.z);
            mainCamera.orthographicSize = maxGridX;

            if(mainCamera.TryGetComponent(out CameraHandler cameraHandler))
            {
                cameraHandler.Initialize();
            }
        }
        #endregion
    }
}