using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] spots;
    public Camera cam;
    int[][] board;
    bool isBlackTurn = true;
    // Start is called before the first frame update
    void Start()
    {
        board = new int[19][];
        for(int i =0; i < board.Length; i++)
        {
            board[i] = new int[19];
        }
        GenerateBoard(board);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),Vector2.zero);
            if (hit)
            {
                GameObject obj = hit.transform.gameObject;
                if (obj.tag == "Spot" && 
                    !obj.transform.GetChild(0).gameObject.activeSelf &&
                    !obj.transform.GetChild(1).gameObject.activeSelf)
                {
                    int index = -1;
                    for(int i=0; i<spots.Length;i++)
                    {
                        if (spots[i].Equals(obj))
                        {
                            index = i;
                            break;
                        }
                    }
                    int row = index / 19;
                    int col = index % 19;
                    if(isBlackTurn)
                    {
                        board[row][col] = 0;
                        if (IsProhibitted(board, row, col))
                        {
                            board[row][col] = -1;
                            return;
                        }
                        obj.transform.GetChild(0).gameObject.SetActive(true);
                        isBlackTurn = false;
                    }
                    else
                    {
                        board[row][col] = 1;
                        obj.transform.GetChild(1).gameObject.SetActive(true);
                        isBlackTurn = true;
                    }
                    if (IsGameOver(board, row, col, isBlackTurn ? 1 : 0))
                    {
                        GameOver(!isBlackTurn);
                    }
                    else
                    {
                        if (IsBoardFull(board))
                        {
                            Debug.Log("Regame");
                        }
                    }
                }
            }
        }
    }

    private bool IsBoardFull(int[][] board)
    {
        for (int i = 0; i < board.Length; i++)
        {
            for (int j = 0; j < board[i].Length; j++)
            {
                if (board[i][j] == -1)
                {
                    return false;
                }
            }
        }
        return true;
    }
    private void GameOver(bool isBlackWin)
    {
        if(isBlackWin)
        {
            Debug.Log("black win");
        }
        else
        {
            Debug.Log("white win");
        }
    }
    private bool IsProhibitted(int[][] board, int row, int col)
    {
        if (IsThreeThree(board, row, col))
        {
            Debug.Log("흑 33 금지");
            return true;
        }
        if (IsFourFour(board, row, col))
        {
            Debug.Log("흑 44 금지");
            return true;
        }
        if (IsMoreThanFive(board, row, col))
        {
            Debug.Log("흑 장목 금지");
            return true;
        }
        return false;
    }
    private bool IsMoreThanFive(int[][] board, int row, int col)
    {
        int count;
        int count_five = 0;
        int count_long = 0;

        // 가로로 5개 넘게 이어졌는지
        count = 0;
        for (int i = col; i < 19; i++)
        {
            if (board[row][i] == 0)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        for (int i = col - 1; i >= 0; i--)
        {
            if (board[row][i] == 0)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        if(count == 5) { count_five++; }
        if (count > 5) { count_long++; }

        // 세로로 5개 넘게 이어졌는지
        count = 0;
        for (int i = row; i < 19; i++)
        {
            if (board[i][col] == 0)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        for (int i = row - 1; i >= 0; i--)
        {
            if (board[i][col] == 0)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        if (count == 5) { count_five++; }
        if (count > 5) { count_long++; }

        // 우상향 대각선으로 이어졌는지
        count = 0;
        for (int i = row, j = col; i >= 0 && j < 19; i--, j++)
        {
            if (board[i][j] == 0)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        for (int i = row + 1, j = col - 1; i < 19 && j >= 0; i++, j--)
        {
            if (board[i][j] == 0)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        if (count == 5) { count_five++; }
        if (count > 5) { count_long++; }

        // 우하향 대각선으로 이어졌는지
        count = 0;
        for (int i = row, j = col; i < 19 && j < 19; i++, j++)
        {
            if (board[i][j] == 0)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        for (int i = row - 1, j = col - 1; i >= 0 && j >= 0; i--, j--)
        {
            if (board[i][j] == 0)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        if (count == 5) { count_five++; }
        if (count > 5) { count_long++; }

        if (count_five > 0) { return false; }
        if (count_long > 0) { return true;  }
        return false;
    }
    private bool IsThreeThree(int[][] board, int row, int col)
    {
        int threeStoneCount = 0;
        int[] line;

        // 가로로 라인 받기
        line = new int[19];
        for (int i = 0; i < 19; i++)
        {
            line[i] = board[row][i];
        }
        if (IsThreeLeft(line,row,col) || IsThreeRight(line, row, col))
        {
            threeStoneCount++;
        }

        // 세로로 라인 받기
        line = new int[19];
        for (int i = 0; i < 19; i++)
        {
            line[i] = board[i][col];
        }
        if (IsThreeLeft(line,row,col) || IsThreeRight(line, row, col))
        {
            threeStoneCount++;
        }

        int index;

        // 우상향 대각선으로 라인 받기
        line = new int[] { -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2 };
        index = col;
        for (int i = row, j = col; i < 19 && j >= 0; i++, j--)
        {
            line[index--] = board[i][j];
        }
        index = col+1;
        for (int i = row-1, j = col+1; i >= 0 && j < 19; i--,j++)
        {
            line[index++] = board[i][j];
        }
        //string tempString = "";
        //foreach (int i in line)
        //{
        //    tempString += i.ToString();
        //}
        //Debug.Log(tempString);
        if (IsThreeLeft(line, row, col) || IsThreeRight(line, row, col))
        {
            threeStoneCount++;
        }

        // 우하향 대각선으로 라인 받기
        line = new int[] { -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2 };
        index = col;
        for (int i = row, j = col; i >= 0 && j >= 0; i--, j--)
        {
            line[index--] = board[i][j];
        }
        index = col+1;
        for (int i = row + 1, j = col + 1; i < 19 && j < 19; i++, j++)
        {
            line[index++] = board[i][j];
        }
        if (IsThreeLeft(line, row, col) || IsThreeRight(line,row,col))
        {
            threeStoneCount++;
        }

        if (threeStoneCount >= 2)
        {
            return true;
        }
        return false;
    }
    private bool IsThreeLeft(int[] line,int row,int col)
    {
        int most_left = col;
        while (most_left >= 0)
        {
            if (line[most_left] == 0) { most_left--; }
            else { break; }
        }
        most_left++;
        if (most_left == 0) { return false; }
        if (line[most_left-1] != -1) { return false; }

        int[] temp = new int[] { -2, -2, -2, -2, -2, -2 };
        for (int i = 0; i < 6; i++)
        {
            if (most_left + i < 19)
                temp[i] = line[most_left + i];
        }

        if (temp[0] == 0 && temp[1] == 0 && temp[2] == 0 && temp[3] == -1)
            return true;
        if (temp[0] == 0 && temp[1] == 0 && temp[2] == -1 && temp[3] == 0 && temp[4] == -1)
            return true;
        if (temp[0] == 0 && temp[1] == -1 && temp[2] == 0 && temp[3] == 0 && temp[4] == -1)
            return true;
        if (temp[0] == 0 && temp[1] == 0 && temp[2] == -1 && temp[3] == -1 && temp[4] == 0 && temp[5] == -1)
            return true;
        if (temp[0] == 0 && temp[1] == -1 && temp[2] == -1 && temp[3] == 0 && temp[4] == 0 && temp[5] == -1)
            return true;
        return false;
    }
    private bool IsThreeRight(int[] line, int row, int col)
    {
        int most_right = col;
        while (most_right < 19)
        {
            if (line[most_right] == 0) { most_right++; }
            else { break; }
        }
        most_right--;
        if (most_right == 18) { return false; }
        if (line[most_right + 1] != -1) { return false; }

        int[] temp = new int[] {-2,-2,-2,-2,-2,-2 };
        for (int i = 0; i < 6; i++)
        {
            if(most_right - i >= 0 )
                temp[i] = line[most_right - i];
        }

        if (temp[0] == 0 && temp[1] == 0 && temp[2] == 0 && temp[3] == -1)
            return true;
        if (temp[0] == 0 && temp[1] == 0 && temp[2] == -1 && temp[3] == 0 && temp[4] == -1)
            return true;
        if (temp[0] == 0 && temp[1] == -1 && temp[2] == 0 && temp[3] == 0 && temp[4] == -1)
            return true;
        if (temp[0] == 0 && temp[1] == 0 && temp[2] == -1 && temp[3] == -1 && temp[4] == 0 && temp[5] == -1)
            return true;
        if (temp[0] == 0 && temp[1] == -1 && temp[2] == -1 && temp[3] == 0 && temp[4] == 0 && temp[5] == -1)
            return true;
        return false;
    }

    private bool IsFourFour(int[][] board, int row, int col)
    {
        int fourStoneCount = 0;
        int[] line;

        // 가로로 라인 받기
        line = new int[19];
        for (int i = 0; i < 19; i++)
        {
            line[i] = board[row][i];
        }
        if (IsFourLeft(line, row, col) || IsFourRight(line, row, col))
        {
            fourStoneCount++;
        }

        // 세로로 라인 받기
        line = new int[19];
        for (int i = 0; i < 19; i++)
        {
            line[i] = board[i][col];
        }
        if (IsFourLeft(line, row, col) || IsFourRight(line, row, col))
        {
            fourStoneCount++;
        }

        int index;

        // 우상향 대각선으로 라인 받기
        line = new int[] { -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2 };
        index = col;
        for (int i = row, j = col; i < 19 && j >= 0; i++, j--)
        {
            line[index--] = board[i][j];
        }
        index = col + 1;
        for (int i = row - 1, j = col + 1; i >= 0 && j < 19; i--, j++)
        {
            line[index++] = board[i][j];
        }
        //string tempString = "";
        //foreach (int i in line)
        //{
        //    tempString += i.ToString();
        //}
        //Debug.Log(tempString);
        if (IsFourLeft(line, row, col) || IsFourRight(line, row, col))
        {
            fourStoneCount++;
        }

        // 우하향 대각선으로 라인 받기
        line = new int[] { -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2 };
        index = col;
        for (int i = row, j = col; i >= 0 && j >= 0; i--, j--)
        {
            line[index--] = board[i][j];
        }
        index = col + 1;
        for (int i = row + 1, j = col + 1; i < 19 && j < 19; i++, j++)
        {
            line[index++] = board[i][j];
        }
        if (IsFourLeft(line, row, col) || IsFourRight(line, row, col))
        {
            fourStoneCount++;
        }

        if (fourStoneCount >= 2)
        {
            return true;
        }
        return false;
    }
    private bool IsFourLeft(int[] line, int row, int col)
    {
        int most_left = col;
        while (most_left >= 0)
        {
            if (line[most_left] == 0) { most_left--; }
            else { break; }
        }
        most_left++;
        if (most_left == 0) { return false; }
        if (line[most_left - 1] != -1) { return false; }

        int[] temp = new int[] { -2, -2, -2, -2, -2, -2 };
        for (int i = 0; i < 6; i++)
        {
            if (most_left + i < 19)
                temp[i] = line[most_left + i];
        }

        if (temp[0] == 0 && temp[1] == 0 && temp[2] == 0 && temp[3] == 0 && temp[4] == -1)
            return true;
        if (temp[0] == 0 && temp[1] == -1 && temp[2] == 0 && temp[3] == 0 && temp[4] == 0 && temp[5] == -1)
            return true;
        if (temp[0] == 0 && temp[1] == 0 && temp[2] == -1 && temp[3] == 0 && temp[4] == 0 && temp[5] == -1)
            return true;
        if (temp[0] == 0 && temp[1] == 0 && temp[2] == 0 && temp[3] == -1 && temp[4] == 0 && temp[5] == -1)
            return true;
        return false;
    }
    private bool IsFourRight(int[] line, int row, int col)
    {
        int most_right = col;
        while (most_right < 19)
        {
            if (line[most_right] == 0) { most_right++; }
            else { break; }
        }
        most_right--;
        if (most_right == 18) { return false; }
        if (line[most_right + 1] != -1) { return false; }

        int[] temp = new int[] { -2, -2, -2, -2, -2, -2 };
        for (int i = 0; i < 6; i++)
        {
            if (most_right - i >= 0)
                temp[i] = line[most_right - i];
        }

        if (temp[0] == 0 && temp[1] == 0 && temp[2] == 0 && temp[3] == 0 && temp[4] == -1)
            return true;
        if (temp[0] == 0 && temp[1] == -1 && temp[2] == 0 && temp[3] == 0 && temp[4] == 0 && temp[5] == -1)
            return true;
        if (temp[0] == 0 && temp[1] == 0 && temp[2] == -1 && temp[3] == 0 && temp[4] == 0 && temp[5] == -1)
            return true;
        if (temp[0] == 0 && temp[1] == 0 && temp[2] == 0 && temp[3] == -1 && temp[4] == 0 && temp[5] == -1)
            return true;
        return false;
    }

    void GenerateBoard(int[][] board)
    {
        for (int i = 0; i< 19; i++) {
            for (int j = 0; j< 19; j++)
            {
                board[i][j] = -1;
            }
        }
    }
    bool IsGameOver(int[][] board, int row, int col, int stone)
    {
        int count;

        // 가로로 5개 이어졌는지
        count = 0;
        for(int i = col; i< 19; i++)
        {
            if (board[row][i] == stone)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        for(int i = col - 1; i>=0; i--)
        {
            if (board[row][i] == stone)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        if(count >= 5)
        {
            return true;
        }

        // 세로로 5개 이어졌는지
        count = 0;
        for (int i = row; i < 19; i++)
        {
            if (board[i][col] == stone)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        for (int i = row - 1; i >= 0; i--)
        {
            if (board[i][col] == stone)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        if (count >= 5)
        {
            return true;
        }

        // 우상향 대각선으로 이어졌는지
        count = 0;
        for (int i = row, j = col; i>=0 && j < 19; i--, j++)
        {
            if (board[i][j] == stone)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        for (int i = row  + 1, j = col - 1; i < 19 && j >= 0; i++, j--)
        {
            if (board[i][j] == stone)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        if (count >= 5)
        {
            return true;
        }

        // 우하향 대각선으로 이어졌는지
        count = 0;
        for (int i = row, j = col; i < 19 && j < 19; i++, j++)
        {
            if (board[i][j] == stone)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        for (int i = row - 1, j = col - 1; i >= 0 && j >= 0; i--, j--)
        {
            if (board[i][j] == stone)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        if (count >= 5)
        {
            return true;
        }

        return false;
    }
}