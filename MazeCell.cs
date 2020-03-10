/*
 * Max Calhoun
 * MazeCell
 * used for creating a randomized maze
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a maze cell used for building a maze
class MazeCell : MonoBehaviour
{
    //Constructor
    public MazeCell()
    {
        left_ = false;
        right_ = false;
        up_ = false;
        down_ = false;
        visited_ = false;
    }

    //accessors
    public bool left() { return left_; }
    public bool right() { return right_; }
    public bool up() { return up_; }
    public bool down() { return down_; }
    public bool visited() { return visited_; }

    //modifiers
    public void setLeft() { left_ = true; }
    public void setRight() { right_ = true; }
    public void setUp() { up_ = true; }
    public void setDown() { down_ = true; }
    public void setVisited() { visited_ = true; }

    //private:
    private bool left_;
    private bool right_;
    private bool up_;
    private bool down_;
    private bool visited_;
}