/*
 * Max Calhoun
 * Maze
 * creates a randomized maze
 */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct cell
{
    public int r, c;

    public cell(int r1, int c1) { r = r1; c = c1; }
}

//builds a maze of mazeHeight_ x mazeWidth_
class Maze : MonoBehaviour
{
    //public:
    public GameObject wall_;
    public GameObject floor_;
    public GameObject tank_;
    public GameObject target_;

    private int mazeWidth_ = 50;
    private int mazeHeight_ = 50;
    private GameObject[,] maze_;
    private GameObject targetManager_;

    // Start is called before the first frame update
    void Start()
    {
        //mazeHeight_ = 10;
        //mazeWidth_ = 10;
        maze_ = new GameObject[mazeHeight_, mazeWidth_];
        targetManager_ = GameObject.Find("TargetManager");
        buildMaze();
    }

    private void buildMaze()
    {
        //Building guide:
        //vector(x, y, z)
        //x corresponds with column. as c increases, x increases
        //z corresponds with row. as r increases, z decreases

        //placing the floors
        for (int r = 0; r < mazeHeight_; ++r)
            for (int c = 0; c < mazeWidth_; ++c)
                maze_[r, c] = Instantiate(floor_, new Vector3(c * 5, 0, r * -5F), new Quaternion(0, 0, 0, 1));

        Stack<GameObject> targets = new Stack<GameObject>();

        //building the maze logically
        Stack<cell> builder = new Stack<cell>();

        //starting location of the maze
        cell start = new cell(0, 0);
        maze_[start.r, start.c].GetComponent<MazeCell>().setVisited();
        builder.Push(start);

        //while !builder.isEmpty()
        while (builder.Count != 0)
        {
            //while current cell has valid neighbor
            while (hasValidNeighbor(builder.Peek()))
            {
                cell curr = builder.Peek();
                List<cell> neighbors = new List<cell>();

                cell left = new cell(curr.r, curr.c - 1);
                if (isValidNeighbor(left))
                    neighbors.Add(left);

                cell right = new cell(curr.r, curr.c + 1);
                if (isValidNeighbor(right))
                    neighbors.Add(right);

                cell up = new cell(curr.r - 1, curr.c);
                if (isValidNeighbor(up))
                    neighbors.Add(up);

                cell down = new cell(curr.r + 1, curr.c);
                if (isValidNeighbor(down))
                    neighbors.Add(down);

                //pick random neighbor and push onto stack
                int rnd = Random.Range(0, neighbors.Count);
                cell next = neighbors[rnd];

                if (next.r == left.r && next.c == left.c)
                {
                    maze_[curr.r, curr.c].GetComponent<MazeCell>().setLeft();
                    maze_[next.r, next.c].GetComponent<MazeCell>().setRight();
                }
                else if (next.r == right.r && next.c == right.c)
                {
                    maze_[curr.r, curr.c].GetComponent<MazeCell>().setRight();
                    maze_[next.r, next.c].GetComponent<MazeCell>().setLeft();
                }
                else if (next.r == up.r && next.c == up.c)
                {
                    maze_[curr.r, curr.c].GetComponent<MazeCell>().setUp();
                    maze_[next.r, next.c].GetComponent<MazeCell>().setDown();
                }
                else if (next.r == down.r && next.c == down.c)
                {
                    maze_[curr.r, curr.c].GetComponent<MazeCell>().setDown();
                    maze_[next.r, next.c].GetComponent<MazeCell>().setUp();
                }

                maze_[next.r, next.c].GetComponent<MazeCell>().setVisited();
                builder.Push(next);
            }

            //adding targets to the maze and incrementing target count
            targets.Push(Instantiate(target_, new Vector3(builder.Peek().c * 5F, 2.5F, builder.Peek().r * -5F), transform.rotation));
            targetManager_.GetComponent<TargetManager>().incrementCount();

            //while current cell has invalid neighbor
            while (builder.Count > 0 && !hasValidNeighbor(builder.Peek()))
                builder.Pop();
        }

        //printMaze();

        Stack<GameObject> walls = new Stack<GameObject>();

        //Placing the walls
        for (int r = 0; r < mazeHeight_; ++r)
            for (int c = 0; c < mazeWidth_; ++c)
            {
                if (!maze_[r, c].GetComponent<MazeCell>().right())
                {
                    var rotation = new Quaternion();
                    rotation.eulerAngles = new Vector3(0, 90, 0);
                    walls.Push(Instantiate(wall_, new Vector3(c * 5 + 2.5F, 2.5F, r * -5F), rotation));
                }
                if (!maze_[r, c].GetComponent<MazeCell>().down())
                {
                    walls.Push(Instantiate(wall_, new Vector3(c * 5, 2.5F, r * -5F - 2.5F), new Quaternion(0, 0, 0, 1)));
                }
            }
        //Top Wall
        for (int r = 0; r < mazeHeight_; ++r)
        {
            var rotation = new Quaternion();
            rotation.eulerAngles = new Vector3(0, 90, 0);
            walls.Push(Instantiate(wall_, new Vector3(-2.5F, 2.5F, r * -5F), rotation));
        }
        //Left Wall
        for (int c = 0; c < mazeWidth_; ++c)
            walls.Push(Instantiate(wall_, new Vector3(c * 5, 2.5F, 2.5F), new Quaternion()));

        Instantiate(tank_, new Vector3(0F, 1, 0F), transform.rotation);
    }

    //if cell is within width and height of maze, and has not been visited
    bool isValidNeighbor(cell neighbor)
    {
        if (neighbor.r >= 0 && neighbor.r < mazeHeight_ && neighbor.c >= 0 && neighbor.c < mazeWidth_ && maze_[neighbor.r, neighbor.c].GetComponent<MazeCell>().visited() == false)
            return true;
        return false;
    }

    //if any neighbors are valid
    bool hasValidNeighbor(cell curr)
    {
        cell left = new cell(curr.r, curr.c - 1);
        if (isValidNeighbor(left))
            return true;

        cell right = new cell(curr.r, curr.c + 1);
        if (isValidNeighbor(right))
            return true;

        cell up = new cell(curr.r - 1, curr.c);
        if (isValidNeighbor(up))
            return true;

        cell down = new cell(curr.r + 1, curr.c);
        if (isValidNeighbor(down))
            return true;

        return false;
    }

    void printMaze()
    {
        for (int r = 0; r < mazeHeight_; ++r)
            for (int c = 0; c < mazeWidth_; ++c)
            {
                string directions = "";
                if (maze_[r, c].GetComponent<MazeCell>().up())
                    directions += "Up-";
                if (maze_[r, c].GetComponent<MazeCell>().down())
                    directions += "Down-";
                if (maze_[r, c].GetComponent<MazeCell>().left())
                    directions += "Left-";
                if (maze_[r, c].GetComponent<MazeCell>().right())
                    directions += "Right";

                Debug.Log("[" + r + ", " + c + "]: " + directions + "   ");
            }
    }
}