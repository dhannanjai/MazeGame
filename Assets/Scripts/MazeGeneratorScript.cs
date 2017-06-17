using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGeneratorScript: MonoBehaviour {

    public bool toCallSolution = false;

    public GameObject wall;
    //*/public GameObject floorPrefab;*/

    public int xSize = 5;
    public int zSize = 5;
    public float wallLength = 1.0f;

    public Transform playerGenerationPoint;
    private bool isItTheFirstMaze = false;

    public Transform[] mazeGenerationPoints;

    public Vector3[] entryPoint;
    public Vector3[] exitPoint;

    private GameObject floor;
    private int totalCellsCount;
    private Cells[] cells ;
    private Vector3 iPos;//storing the initial position. 

    private GameObject wallPlaceHolder; // This acts as a container to store all the walls in Hierarchy.
    private GameObject tempWall;
    private GameObject[] allWalls;

    bool startedBuilding;

    private int extremeLeftCellNumber;
    private int currentCellNumber;
    private int visitedCellCount;

    private int length;
    private int[] neighbours;
    private int currentNeighbour;
    private int wallToBreak;

    private List<int> lastCells;
    private int TopOfStack;

    private void Awake()
    {
        if (xSize <= 1)
            xSize = 2;
        if (zSize <= 1)
            zSize = 2;

        ///definition of cells array.
        totalCellsCount = xSize * zSize;
        cells = new Cells[totalCellsCount];

        entryPoint = new Vector3[mazeGenerationPoints.Length];
        exitPoint = new Vector3[mazeGenerationPoints.Length];

        for (int i=0;i<mazeGenerationPoints.Length;i++)
        {
            isItTheFirstMaze = (i == 0) ? true : false;
            visitedCellCount = 0;
            startedBuilding = false;
            iPos = new Vector3(
                                (mazeGenerationPoints[i].transform.position.x-xSize / 2) + wallLength / 2, 
                                0.0f,
                                (mazeGenerationPoints[i].transform.position.z- zSize / 2) + wallLength / 2
                              );
            createGrid();
            CreateEntryAndExit(i);
        }
        
    }

    void createGrid()
    {
        wallPlaceHolder = new GameObject();
        wallPlaceHolder.name = "The Grid";
        Vector3 newPos;
        //along the x-axis
        for(int i=0;i<zSize;i++)
            for(int j=0;j<=xSize;j++)
            {
                newPos = new Vector3(iPos.x + j * wallLength - wallLength / 2, 0.0f, iPos.z + i * wallLength - wallLength / 2);
                tempWall = Instantiate(wall, newPos, Quaternion.identity) as GameObject;
                tempWall.transform.parent = wallPlaceHolder.transform; 
            }

        //along the z-axis
        for (int i = 0; i <= zSize; i++)
            for (int j = 0; j < xSize; j++)
            {
                newPos = new Vector3(iPos.x + j * wallLength, 0.0f, iPos.z + (i * wallLength) - wallLength);
                tempWall = Instantiate(wall, newPos, Quaternion.Euler(0f,90f,0f)) as GameObject;
                tempWall.transform.parent = wallPlaceHolder.transform;
            }

        CreateCells();
    }

    void CreateCells()
    {
        lastCells = new List<int>();
        lastCells.Clear();
        
       
        int childCount = wallPlaceHolder.transform.childCount;
        allWalls = new GameObject[childCount];
        
        for (int i = 0; i < childCount; i++)
            allWalls[i] = wallPlaceHolder.transform.GetChild(i).gameObject;
     
        for(int i = 0 ; i < cells.Length ; i++)
        {
            cells[i] = new Cells();
            int down = i + (xSize + 1) * zSize;
            int up = down + xSize;
            int rowc = i / xSize;
            int left = rowc * (xSize + 1) + (i-xSize*rowc);
            int right = left + 1;

            cells[i].down = allWalls[down];
            cells[i].up = allWalls[up];
            cells[i].left = allWalls[left];
            cells[i].right = allWalls[right];
        }

        CreateMaze();
        //CreateFloor();

        ///assign first wall transform to the variable PlayerGpoint in GameManagerScript.cs:24 in line 33
        if(isItTheFirstMaze)
            playerGenerationPoint = cells[0].left.transform;
    }

/// <summary>
    /// This function will use depth first search in order to create a maze.
    /// 1.  check that if the number of visited cells is greater than the total size of the grid so created. yes->2 else 5
    /// 2.  Now it checks that whether if maze buiilding has started or not. yes->a else 3.
    ///     a. You have to look for a neighbour or get the last cell visited(explained later).
    ///     b. Check if the current neighbour is ! visited and current cell is visited. yes->i. 
    ///         i.  Break the wall 'Wall to break' of cuurent cell number.
    ///         ii. set current neighbour to be visited.         ///--> are same
    ///   --->  iii. increase the count of number of visited.   
    ///         iv. add the current cell number to list of visited cells.
    ///         v.  set cuurent neighbour to current cell now.
    ///         vi. update top of stack.
    ///     
    /// 3.  
    ///         i.  Select any random cell.
    ///         ii. declare that we have started  building the maze by setting started building to true.
    ///         iii.set current visited cell to true.
    ///   --->  iv.increment the count of visited cell.
    ///   
    /// 4.  Go to step 1
    /// 5.  Call entry and exit Point function.
    /// 6.  Exit
    /// </summary>
    void CreateMaze()
    {
        while (visitedCellCount < totalCellsCount)
        {
            if (startedBuilding)
            {
                GiveMeNeighbour();

                if (cells[currentNeighbour].visited != true && cells[currentCellNumber].visited == true) ///In case the current cell is chosen from the stack
                {
                    BreakWall(currentCellNumber,wallToBreak);
                    cells[currentNeighbour].visited = true;
                    visitedCellCount++;
                    lastCells.Add(currentCellNumber);
                    currentCellNumber = currentNeighbour;

                    if (lastCells.Count >= 0)
                    {
                        TopOfStack = lastCells.Count - 1;
                    }
                }
            }
            else
            {
                currentCellNumber = Random.Range(0, totalCellsCount);
                startedBuilding = true;
                cells[currentCellNumber].visited = true;
                visitedCellCount++;
                
            }
        }
        /*
        if (gameObject.GetComponent<DjikstraSolveMaze>() != null && toCallSolution == true)
        {
            gameObject.GetComponent<DjikstraSolveMaze>().called();
        }
        */
    }

/// <summary>
/// This gives the neighbour of the cuurent cell or the last visited cell.
/// 
/// 1.  set length=0,
///     define neighbours to be an array of four int(carry the neighbpur cellnumber),
///     and same as for the connecting wall(carry the wall to break number wrt current wall)
/// 2.  calculate the cell number for the current cell number.
/// 3.  check for right,left,up and down.
///             Checks:-
///                 Right : check if the next cell is ! in left extreme and is less then the total size of the grid.
///                 Left  : Check if the prvious cell is 0 and if it is a left extreme.
///                 up  :   check if cell no. + xSize less than total cell count 
///                 down:   check if cell no - xSize is greater than or = Zero
///     if true then check if they are not visited , true then
///         a. set neighbour[length] = the wall
///         b.  connectingWall[length] = wall number.
///         c.length++
///  4. if length! = 0, then a below elseb
///     a.    select any random from length.
///         Set currentNeighbour to Neighbours[thechosenone]  (which carry the cell numbers)
///         Set walltoBreak = connectinWall[theChosenOne]
///     b. Select cell to be the last cell visited
///         decrease the stack
///                 
/// </summary>
    void GiveMeNeighbour()
    {
        length = 0;
        neighbours = new int[4];
        int[] connectingWall = new int[4];

        extremeLeftCellNumber = ((currentCellNumber + 1) / xSize) * xSize;

        //right
        if (currentCellNumber + 1 < totalCellsCount && currentCellNumber + 1 != extremeLeftCellNumber)
        {
            if (!cells[currentCellNumber + 1].visited)
            {
                neighbours[length] = currentCellNumber + 1;
                connectingWall[length] = 4;
                length++;
            }
        }

        //left
        if (currentCellNumber != extremeLeftCellNumber)
        {
            if (!cells[currentCellNumber - 1].visited)
            {
                neighbours[length] = currentCellNumber - 1;
                connectingWall[length] = 3;
                length++;
            }
        }

        //up
        if (currentCellNumber + xSize < totalCellsCount)
        {
            if (!cells[currentCellNumber + xSize].visited)
            {
                neighbours[length] = currentCellNumber + xSize;
                connectingWall[length] = 1;
                length++;
            }
        }

        //down
        if (currentCellNumber - xSize >= 0)
        {
            if (!cells[currentCellNumber - xSize].visited)
            {
                neighbours[length] = currentCellNumber - xSize;
                connectingWall[length] = 2;
                length++;
          
            }
        }
        if (length != 0)
        {
            int chosenOne=Random.Range(0, length);
            currentNeighbour = neighbours[chosenOne];
            wallToBreak = connectingWall[chosenOne];

        }
        else
        {
            if (TopOfStack > 0)
            {
                currentCellNumber = lastCells[TopOfStack];
                TopOfStack--;
            }
        }
    }

/// <summary>
/// Breaks the wall of the given cell number and the wall number.
/// </summary>
/// <param name="_currentCellNumber"></param>
/// <param name="_wallToBreak"></param>
    void BreakWall(int _currentCellNumber, int _wallToBreak)
    {
        switch(_wallToBreak)
        {
            case 1:
                Destroy(cells[_currentCellNumber].up);
                cells[_currentCellNumber].up = null;
                break;
            case 2:
                Destroy(cells[_currentCellNumber].down);
                cells[_currentCellNumber].down = null;
                break;
            case 3:
                Destroy(cells[currentCellNumber].left);
                cells[_currentCellNumber].left = null;
                break;
            case 4:
                Destroy(cells[_currentCellNumber].right);
                cells[_currentCellNumber].down = null;
                break;
        }
    }

/// <summary>
/// Destroys the left wall of the first cell and right wall of the last cell.
/// </summary>
    void CreateEntryAndExit(int i)
    {
        currentCellNumber = 0;
        entryPoint[i] = cells[currentCellNumber].left.gameObject.transform.position + Vector3.left/2 ;
        BreakWall(currentCellNumber, 3);
        
        currentCellNumber = totalCellsCount-1;
        exitPoint[i] = cells[currentCellNumber].right.gameObject.transform.position + Vector3.right / 2;
        BreakWall(currentCellNumber, 4);
        
    }

    /*
     * Not needed right now...
     
    void CreateFloor()
    {
        Vector3 floorposition = new Vector3(0f, -0.5f, 0f);
         floor = Instantiate(floorPrefab,floorposition , Quaternion.identity) as GameObject;
        floor.transform.localScale = new Vector3(xSize/10f +0.2f, 1, zSize/10f+0.2f);
    }
    */
}
