using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game
{
    class Program
    {
        static void Main(string[] args)
        {
            Cell[,] board = new Cell[8, 8];
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    board[i, j] = new Cell(i, j);
                }
            }

            // Creating Units and alocating them on board
            Pawn[] unit = new Pawn[16];

            // Creating Powns 
            bool color = true;
            for (int i = 0, column = 0; i < 16; i++, column++)
            {            
                if (column == 8)
                {
                    color = false;
                    column = 0;
                }
                unit[i] = new Pawn(color ? "PW" : "PB", color);
                if (color)
                {
                    board[1, column].setSoldier(unit[i]);
                    board[1, column].getSoldier().setRow(1);
                    board[1, column].getSoldier().setColumn(column);
                }
                else
                {
                    board[6, column].setSoldier(unit[i]);
                    board[6, column].getSoldier().setRow(6);
                    board[6, column].getSoldier().setColumn(column);
                }

            }
            // Creating Rooks 
            Rook rookW1 = new Rook("RW", true); rookW1.setRow(0); rookW1.setColumn(0); board[0, 0].setSoldier(rookW1);
            Rook rookW2 = new Rook("RW", true); rookW2.setRow(0); rookW2.setColumn(7); board[0, 7].setSoldier(rookW2);
            Rook rookB1 = new Rook("RB", false); rookB1.setRow(7); rookB1.setColumn(0); board[7, 0].setSoldier(rookB1);
            Rook rookB2 = new Rook("RB", false); rookB2.setRow(7); rookB2.setColumn(7); board[7, 7].setSoldier(rookB2);
            // Creating Knights
            Knight knightW1 = new Knight("NW", true); knightW1.setRow(0); knightW1.setColumn(1); board[0, 1].setSoldier(knightW1);
            Knight knightW2 = new Knight("NW", true); knightW2.setRow(0); knightW2.setColumn(6); board[0, 6].setSoldier(knightW2);
            Knight knightB1 = new Knight("NB", false); knightB1.setRow(7); knightB1.setColumn(1); board[7, 1].setSoldier(knightB1);
            Knight knightB2 = new Knight("NB", false); knightB2.setRow(7); knightB2.setColumn(6); board[7, 6].setSoldier(knightB2);
            // Creating Bishops
            Bishop bishopW1 = new Bishop("BW", true); bishopW1.setRow(0); bishopW1.setColumn(2); board[0, 2].setSoldier(bishopW1);
            Bishop bishopW2 = new Bishop("BW", true); bishopW2.setRow(0); bishopW2.setColumn(5); board[0, 5].setSoldier(bishopW2);
            Bishop bishopB1 = new Bishop("BB", false); bishopB1.setRow(7); bishopB1.setColumn(2); board[7, 2].setSoldier(bishopB1);
            Bishop bishopB2 = new Bishop("BB", false); bishopB2.setRow(7); bishopB2.setColumn(5); board[7, 5].setSoldier(bishopB2);

            // Creating Kings and Queens
            Queen queenW = new Queen("QW", true); queenW.setRow(0); queenW.setColumn(3); board[0, 3].setSoldier(queenW);
            Queen queenB = new Queen("QB", false); queenB.setRow(7); queenB.setColumn(3); board[7, 3].setSoldier(queenB);
            King kingB = new King("KB", false); board[7, 4].setSoldier(kingB);
            King kingW = new King("KW", true); board[0, 4].setSoldier(kingW);


            // Making a move
            int moveCount = 0;
            bool gameEnded = false;
            bool moveSuccess;
            int[] nonePawnMoveCount = new int[1];
            nonePawnMoveCount[0] = 0;
            

            string[] positions =new string[1000];
            samePositionThreeTimes(board, moveCount, positions);

            print(board,kingW,kingB);

            while (!gameEnded)
            {
                string messege = moveCount % 2 == 0 ? "White's turn" : "Blacks turn";
                Console.WriteLine(messege);
                Console.WriteLine("Who you want to move");
                string oldPosition = Console.ReadLine();
                Console.WriteLine("Where you want to move");
                string newPosition = Console.ReadLine();

                // Check if move possible
                moveSuccess = move(oldPosition.Trim(), newPosition.Trim(), board, moveCount, kingW, kingB, nonePawnMoveCount);
                moveCount = moveSuccess? ++moveCount: moveCount;

                print(board, kingW, kingB);
                // Check game end
                if (moveSuccess)
                {
                    if (kingW.getIsInDanger())
                        if (!kingW.canEscape(board) && !(kingW.isBlockPossible(board)))
                        {
                            Console.WriteLine("Black Won!");
                            break;
                        }
                    if (kingB.getIsInDanger())
                        if (!kingB.canEscape(board) && !(kingB.isBlockPossible(board)))
                        {
                            Console.WriteLine("White Won!");
                            break;
                        }
                    //check for Pat
                    bool isWhite;
                    if (!kingW.getIsInDanger() && kingW.canEscape(board))
                    {
                        isWhite = true;
                        if (!checkPat(board, isWhite, kingW))
                        {
                            Console.WriteLine("It's a Tie!, white has no moves available");
                            break;
                        }
                    }
                    if (!kingB.getIsInDanger() && kingB.canEscape(board))
                    {
                        isWhite = false;
                        if (!checkPat(board, isWhite, kingB))
                        {
                            Console.WriteLine("It's a Tie!, black has no moves available");
                            break;
                        }
                    }
                    //check for tie due to lack of units
                    if (checkTieByLackOfUnits(board))
                        break;
                    if (nonePawnMoveCount[0] == 50)
                    {
                        Console.WriteLine("It's a Tie, no Pawn were moved for 50 turns");
                        break;
                    }
                    if (samePositionThreeTimes(board, moveCount, positions))
                    {
                        Console.WriteLine("It's a Tie, same position 3 times");
                        break;
                    }
                }
                
            }
        }
        public static bool samePositionThreeTimes(Cell[,] board, int moveCount, string[] positions)
        {
            int count1 = 0;
            int count2 = 0;
            int duplicatesW = 0;
            int duplicatesB = 0;
           
            positions[moveCount] = "";
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j].getSoldier() == null)
                        positions[moveCount] += "EE";
                    else
                        positions[moveCount] += board[i, j].getSoldier().getName();
                }
            while (count1 <= moveCount)
            {
                while (count2 <= moveCount)
                {
                    if (positions[count1] == positions[count2] && count2 % 2 == 0)
                        duplicatesW++;
                    if (positions[count1] == positions[count2] && count2 % 2 != 0)
                        duplicatesB++;
                    count2++;
                }
                if (duplicatesW == 3 || duplicatesB == 3)
                    return true;
                else
                {
                    duplicatesW = 0;
                    duplicatesB = 0;
                }
                count2 = 0;
                count1++;
            }



            return false;
        }
        public static bool checkTieByLackOfUnits (Cell[,] board)
        {
            int whiteUnitsWeight = 0;
            int blackUnitsWeight = 0;

            for (int i = 0; i < board.GetLength(0); i ++)
                for(int j = 0; j < board.GetLength(1); j++)
                {
                    if(board[i,j].getSoldier() != null)
                    {
                        if (board[i, j].getSoldier() is Pawn)
                            return false;
                        if (board[i, j].getSoldier() is Rook)
                            return false;
                        if (board[i, j].getSoldier() is Queen)
                            return false;
                        switch (board[i,j].getSoldier().getIsWhite())
                        {
                            case true:
                                whiteUnitsWeight++;
                                break;
                            case false:
                                blackUnitsWeight++;
                                break;
                        }
                        if (blackUnitsWeight > 2 || whiteUnitsWeight > 2)
                            return false;                       
                    }
                }
            Console.WriteLine("It's a Tie, not enough units left on the board");
            return true;
                    
        }
        public static bool checkPat (Cell[,] board, bool isWhite, King king)
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j].getSoldier() != null)
                        if (board[i, j].getSoldier().getIsWhite() == isWhite)
                        {
                            if ( board[i,j].getSoldier() is Pawn)
                            {
                                Pawn other = (Pawn)board[i, j].getSoldier();                                
                                if (other.setMovePossible(board, king))
                                    return true;
                            }
                            if (board[i, j].getSoldier() is Knight)
                            {
                                Knight other = (Knight)board[i, j].getSoldier();
                                if (other.setMovePossible(board, king))
                                    return true;
                            }
                            if (board[i, j].getSoldier() is Bishop)
                            {
                                Bishop other = (Bishop)board[i, j].getSoldier();
                                if (other.setMovePossible(board, king))
                                    return true;
                            }
                            if (board[i, j].getSoldier() is Rook)
                            {
                                Rook other = (Rook)board[i, j].getSoldier();
                                if (other.setMovePossible(board, king))
                                    return true;
                            }
                            if (board[i, j].getSoldier() is Queen)
                            {
                                Queen other = (Queen)board[i, j].getSoldier();
                                if (other.setMovePossible(board, king))
                                    return true;
                            }
                            if (board[i, j].getSoldier() is King)
                            {
                                King other = (King)board[i, j].getSoldier();
                                if (other.setMovePossible(board, king))
                                    return true;
                            }
                        }
                }
            }
            return false;

        }

        public static bool move(string oldPosition, string newPosition, Cell[,] board, int moveCount, King kingW, King kingB, int[] nonePawnMoveCount)
        {
            int oldRow, newRow;
            int oldColumn = 0, newColumn = 0;
            string raw;
            bool legalMove = true;
  
            if (oldPosition.Length != 2)
            {
                Console.WriteLine("Wrong Input");
                return false;
            }

            if (newPosition.Length != 2)
            {
                Console.WriteLine("Wrong Input");
                return false;
            }

            // Getting cordinates
            switch (oldPosition[1])
            {
                case 'A': case 'a':
                    oldColumn = 0;
                    break;
                case 'B': case 'b':
                    oldColumn = 1;
                    break;
                case 'C': case 'c':
                    oldColumn = 2;
                    break;
                case 'D': case 'd':
                    oldColumn = 3;
                    break;
                case 'E': case 'e':
                    oldColumn = 4;
                    break;
                case 'F': case 'f':
                    oldColumn = 5;
                    break;
                case 'G': case 'g':
                    oldColumn = 6;
                    break;
                case 'H': case 'h':
                    oldColumn = 7;
                    break;
                default:
                    Console.WriteLine("Wrong Input");
                    return false;
            }
            switch (oldPosition[0])
            {
                case '1': case '2': case '3': case '4': case '5': case '6': case '7': case '8':
                    raw = "" + oldPosition[0];
                    break;
                default:
                    Console.WriteLine("Wrong Input");
                    return false;
            }
            
            oldRow = int.Parse(raw) - 1;
            switch (newPosition[1])
            {
                case 'A': case 'a':
                    newColumn = 0;
                    break;
                case 'B': case 'b':
                    newColumn = 1;
                    break;
                case 'C': case 'c':
                    newColumn = 2;
                    break;
                case 'D': case 'd':
                    newColumn = 3;
                    break;
                case 'E': case 'e':
                    newColumn = 4;
                    break;
                case 'F': case 'f':
                    newColumn = 5;
                    break;
                case 'G': case 'g':
                    newColumn = 6;
                    break;
                case 'H': case 'h':
                    newColumn = 7;
                    break;
                default:
                    Console.WriteLine("Wrong Input");
                    return false;
            }
            switch (newPosition[0])
            {
                case '1':  case '2':  case '3':  case '4':  case '5':  case '6': case '7': case '8':
                    raw = "" + newPosition[0];
                    break;
                default:
                    Console.WriteLine("Wrong Input");
                    return false;
            }
            newRow = int.Parse(raw) - 1;

            Cell currentCell = board[oldRow, oldColumn];
            Cell newCell = board[newRow, newColumn];
            ChessPiece troopToMove = currentCell.getSoldier();
            ChessPiece TroopToKIll = newCell.getSoldier();

            // Check for empty space
            if (currentCell.getSoldier() == null)
            {
                Console.WriteLine("Wrong input,there is no unit here");
                return false;
            }

            // Check for right color unit
            if(moveCount % 2 == 0 && currentCell.getSoldier().getIsWhite() == false)
            {
                Console.WriteLine("Wrong input, white's move");
                return false;
            }
            if(moveCount % 2 != 0 && currentCell.getSoldier().getIsWhite() == true)
            {
                Console.WriteLine("Wrong input, black's move");
                return false;
            }

            // Check if the move us legal          
            if (currentCell.getSoldier() is Rook)
            {
                Rook other = (Rook)currentCell.getSoldier();
                legalMove = other.legalMove(board, oldRow, oldColumn, newRow, newColumn);
            }
            if (currentCell.getSoldier() is Knight)
            {
                Knight other = (Knight)(currentCell.getSoldier());
                legalMove = other.legalMove(board, oldRow, oldColumn, newRow, newColumn);
            }
            if (currentCell.getSoldier() is Bishop)
            {
                Bishop other = (Bishop)(currentCell.getSoldier());
                legalMove = other.legalMove(board, oldRow, oldColumn, newRow, newColumn);
            }
            if (currentCell.getSoldier() is Queen)
            {
                Queen other = (Queen)(currentCell.getSoldier());
                legalMove = other.legalMove(board, oldRow, oldColumn, newRow, newColumn);
            }
            if (currentCell.getSoldier() is King)
            {
                King other = (King)(currentCell.getSoldier());
                legalMove = other.legalMove(board, oldRow, oldColumn, newRow, newColumn);
            }
            if (currentCell.getSoldier() is Pawn)
            {
                Pawn other = (Pawn)(currentCell.getSoldier());
                legalMove = other.legalMove(board, oldRow, oldColumn, newRow, newColumn, moveCount);

                //If moved Check and it's last line Queen Created 
                if (legalMove && other.getIsWhite() == true && newRow == 7)
                {
                    bool isUnitPlaced = false;
                    while (!isUnitPlaced)
                    {
                        Console.WriteLine("Place new unit");
                        Console.WriteLine("Q R K B");
                        string input = Console.ReadLine();
                        switch (input.Trim())
                        {
                            case "q":
                            case "Q":
                                Queen newQueen = new Queen("QW", true);
                                currentCell.setSoldier(newQueen);
                                isUnitPlaced = true;
                                break;
                            case "r":
                            case "R":
                                Rook newRook = new Rook("RW", true);
                                currentCell.setSoldier(newRook);
                                isUnitPlaced = true;
                                break;
                            case "n":
                            case "N":
                                Knight newKnight = new Knight("NW", true);
                                currentCell.setSoldier(newKnight);
                                isUnitPlaced = true;
                                break;
                            case "b":
                            case "B":
                                Bishop newBishop = new Bishop("BW", true);
                                currentCell.setSoldier(newBishop);
                                isUnitPlaced = true;
                                break;
                            default:
                                Console.WriteLine("Please enter proper unit to place");
                                break;
                        }
                    }

                }
                if (legalMove && other.getIsWhite() == false && newRow == 0)
                {
                    bool isUnitPlaced = false;
                    while (!isUnitPlaced)
                    {
                        Console.WriteLine("Place new unit");
                        Console.WriteLine("Q R N B");
                        string input = Console.ReadLine();
                        switch (input.Trim())
                        {
                            case "q":
                            case "Q":
                                Queen newQueen = new Queen("QB", false);
                                currentCell.setSoldier(newQueen);
                                isUnitPlaced = true;
                                break;
                            case "r":
                            case "R":
                                Rook newRook = new Rook("RB", false);
                                currentCell.setSoldier(newRook);
                                isUnitPlaced = true;
                                break;
                            case "n":
                            case "N":
                                Knight newKnight = new Knight("NB", false);
                                currentCell.setSoldier(newKnight);
                                isUnitPlaced = true;
                                break;
                            case "b":
                            case "B":
                                Bishop newBishop = new Bishop("BB", false);
                                currentCell.setSoldier(newBishop);
                                isUnitPlaced = true;
                                break;
                            default:
                                Console.WriteLine("Please enter proper unit to place");
                                break;
                        }
                    }
                }
                if (legalMove)
                    nonePawnMoveCount[0] = -1;
            }
            if (!legalMove)
            {
                Console.WriteLine("Wrong input");
                return false;
            }
            if (legalMove)
            {
                newCell.setSoldier(currentCell.getSoldier());
                newCell.getSoldier().setRow(newRow);
                newCell.getSoldier().setColumn(newColumn);
                if (currentCell.getSoldier() != null)
                    nonePawnMoveCount[0] = -1;
                currentCell.setSoldier(null);

                //check if king is Checked

                kingW.setIsInDanger(board);
                kingB.setIsInDanger(board);
                if (moveCount % 2 == 0)
                    legalMove = !(kingW.getIsInDanger());
                else
                    legalMove = !(kingB.getIsInDanger());
            }

            if (!legalMove)
            {
                currentCell.setSoldier(troopToMove);
                currentCell.getSoldier().setRow(oldRow);
                currentCell.getSoldier().setColumn(oldColumn);
                newCell.setSoldier(TroopToKIll);
                Console.WriteLine("You Cannot expose your King!");
                if (currentCell.getSoldier() is Rook)
                {
                    Rook other = (Rook)currentCell.getSoldier();
                    other.setNotMoved();
                }
            }
            nonePawnMoveCount[0]++;
            return legalMove;
        }
        public static void print(Cell[,] cells, King kingW, King kingB)
        {
            kingW.setIsInDanger(cells);
            kingB.setIsInDanger(cells);
            if (kingB.getIsInDanger())
                Console.WriteLine("Black King is Checked");
            if (kingW.getIsInDanger())
                Console.WriteLine("White King is Checked");
            
            Console.Write("   =========================================\n");
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                Console.Write(i + 1 + "  | ");
                for (int j = 0; j < cells.GetLength(1); j++)
                    Console.Write(cells[i, j].ToString() + " | ");
                Console.WriteLine("\n   =========================================");   
            }
            Console.Write("      A    B    C    D    E    F    G    H\n");

        }
    }   
    class Cell
    {
        int row;
        int column;
        ChessPiece soldier;
        public Cell() { }
        public Cell (int row, int column)
        {
            setRow(row);
            setColumn(column);
        }
        public void setRow(int row)
        {
            this.row = row;
        }
        public int getRow()
        {
            return row;
        }
        public void setColumn(int column)
        {
            this.column = column;
        }
        public int getColumn()
        {
            return column;
        }
        public override string ToString()
        {
            if (soldier == null)
                return "  ";
            return soldier.getName();
        }
        public virtual void setSoldier(ChessPiece soldier)
        {
            this.soldier = soldier;
        }
        public virtual ChessPiece getSoldier()
        {
            return soldier;
        }

    }
    class ChessPiece : Cell
    {
        string name;
        bool isWhite;
        public ChessPiece(string name, bool isWhite)
        {
            setName(name);
            setIsWhite(isWhite);
        }
        public void setName(string name)
        {
            this.name = name;
        }
        public string getName()
        {
            return name;
        }
        public void setIsWhite(bool isWhite)
        {
            this.isWhite = isWhite;
        }
        public bool getIsWhite()
        {
            return isWhite;
        }
        public override string ToString()
        {
            return name;
        }
        public override ChessPiece getSoldier()
        {
            return base.getSoldier();
        }
        public override void setSoldier(ChessPiece soldier)
        {
            base.setSoldier(soldier);
        }
        public virtual bool setMovePossible (Cell[,] cells, King king)
        {
            return true;
        }

    }
    class Pawn : ChessPiece
    {
        bool isLegalMove;
        bool firstMove;
        bool enPassant;
        int movePassant;
        public bool getEnPassant()
        {
            return enPassant;
        }
        public Pawn (string name, bool isWhite):base (name, isWhite) { firstMove = true;}
        public bool legalMove (Cell[,] board, int oldRow, int oldColumn, int newRow, int newColumn, int countMove)
        {
            Pawn other;
            
            switch (board[oldRow, oldColumn].getSoldier().getIsWhite())
            {
                // case white pawn
                case true:
                    this.firstMove = this.getRow() == 1 ? true : false;
                    // check if the move is withing 1 tile of his position
                    switch (oldColumn == newColumn || oldColumn == newColumn + 1 || oldColumn == newColumn -1)
                    {
                        case true:

                            switch(oldColumn == newColumn)
                            {
                                // Checking regular move
                                case true:
                                    switch (newRow - oldRow)
                                    {
                                        case 1:
                                            isLegalMove = board[newRow, newColumn].getSoldier() == null ? true : false;
                                            break;
                                        case 2:
                                            isLegalMove = board[oldRow + 1, newColumn].getSoldier() == null && board[newRow, newColumn].getSoldier() == null && firstMove? true : false;
                                            if (isLegalMove)
                                            {
                                                this.enPassant = true;
                                                this.movePassant = countMove;
                                                this.firstMove = false;
                                                return true;
                                            }
                                            break;
                                        default:
                                            return false;
                                    }
                                    break;
                                // Checking eating move
                                case false:
                                    switch (newRow - oldRow)
                                    {
                                        case 1:
                                            isLegalMove = board[newRow, newColumn].getSoldier() == null? false : true;
                                            if(!isLegalMove)
                                            {
                                                if(board[oldRow,newColumn].getSoldier() is Pawn)
                                                {
                                                    other = (Pawn)board[oldRow, newColumn].getSoldier();
                                                    if (other.getEnPassant() && countMove == other.movePassant + 1)
                                                    {
                                                        board[oldRow, newColumn].setSoldier(null);
                                                        return true;
                                                    }

                                                }
                                            }
                                            if (!isLegalMove)
                                                return false;
                                            isLegalMove = board[newRow, newColumn].getSoldier().getIsWhite() == true ? false : true;
                                            break;
                                        default:
                                            return false;
                                    }
                                    break;
                            }
                            break;
                        case false:
                            return false;

                    }
                    break;
            
                // Case black pawn
                case false:
                    this.firstMove = this.getRow() == 6 ? true : false;
                    // check if the move is withing 1 tile of his position
                    switch (oldColumn == newColumn || oldColumn == newColumn + 1 || oldColumn == newColumn - 1)
                    {
                        case true:
                            // Checking regular move
                            switch (oldColumn == newColumn)
                            {
                                case true:
                                    switch (newRow - oldRow)
                                    {
                                        case -1:
                                            isLegalMove = board[newRow, newColumn].getSoldier() == null ? true : false;
                                            break;
                                        case -2:
                                            isLegalMove = board[oldRow - 1, newColumn].getSoldier() == null && board[newRow, newColumn].getSoldier() == null && firstMove ? true : false;
                                            if (isLegalMove)
                                            {
                                                this.enPassant = true;
                                                this.movePassant = countMove;
                                                this.firstMove = false;
                                                return true;
                                            }
                                            break;
                                        default:
                                            return false;
                                    }
                                    break;
                                // Checking eating move
                                case false:
                                    switch (newRow - oldRow)
                                    {
                                        case - 1:
                                            isLegalMove = board[newRow, newColumn].getSoldier() == null ? false : true;
                                            if (!isLegalMove)
                                            {
                                                if (board[oldRow, newColumn].getSoldier() is Pawn)
                                                {
                                                    other = (Pawn)board[oldRow, newColumn].getSoldier();
                                                    if (other.getEnPassant() && countMove == other.movePassant + 1)
                                                    {
                                                        board[oldRow, newColumn].setSoldier(null);
                                                        return true;
                                                    }

                                                }
                                            }
                                            if (!isLegalMove)
                                                return false;
                                            isLegalMove = board[newRow, newColumn].getSoldier().getIsWhite() == true ? true : false;
                                            break;
                                        default:
                                            return false;
                                    }
                                    break;
                            }
                            break;
                        case false:
                            return false;

                    }
                    break;            
            }
            this.firstMove = false;
            this.enPassant = false;
            return isLegalMove;

        }
        public override bool setMovePossible(Cell[,] cells, King king)
        {
            int row = this.getRow();
            int column = this.getColumn();
            Pawn other = (Pawn)cells[row, column].getSoldier();

            cells[row, column].setSoldier(null);
            if (!king.setIsInDanger(cells))
            {
                cells[row, column].setSoldier(other);
                return false;
            }
            switch (other.getIsWhite())
            {
                case true:
                    if (cells[row+1,column].getSoldier() == null)
                    {
                        cells[row, column].setSoldier(other);
                        return true;
                    }
                    if (column !=7)
                    {
                        if (cells[row + 1, column + 1].getSoldier() != null)
                            if (cells[row + 1, column + 1].getSoldier().getIsWhite() != this.getIsWhite())
                            {
                                cells[row, column].setSoldier(other);
                                return true;
                            }
                    }
                    if (column != 0)
                    {
                        if (cells[row + 1, column - 1].getSoldier() != null)
                            if (cells[row + 1, column - 1].getSoldier().getIsWhite() != this.getIsWhite())
                            {
                                cells[row, column].setSoldier(other);
                                return true;
                            }
                    }


                    break;

                case false:
                    if (cells[row - 1, column].getSoldier() == null)
                    {
                        cells[row, column].setSoldier(other);
                        return true;
                    }
                    if (column != 7)
                    {
                        if (cells[row - 1, column + 1].getSoldier() != null)
                            if (cells[row - 1, column + 1].getSoldier().getIsWhite() != this.getIsWhite())
                            {
                                cells[row, column].setSoldier(other);
                                return true;
                            }
                    }
                    if (column !=0)
                    {
                        if (cells[row - 1, column - 1].getSoldier() != null)
                            if (cells[row - 1, column - 1].getSoldier().getIsWhite() != this.getIsWhite())
                            {
                                cells[row, column].setSoldier(other);
                                return true;
                            }
                    }

                    break;
            }

            cells[row, column].setSoldier(other);
            return false;
        }
    }
    class Rook : ChessPiece
    {
        bool isLegalMove;
        bool isMoved;
        public Rook(string name, bool isWhite) : base(name, isWhite) { }
        public bool legalMove (Cell[,] cells, int oldRow, int oldColumn, int newRow, int newColumn)
        {
            int distance;
            isLegalMove = false;

            // check if Destination is Clear or enemy
            if (cells[newRow, newColumn].getSoldier() == null || cells[newRow, newColumn].getSoldier().getIsWhite() != cells[oldRow, oldColumn].getSoldier().getIsWhite())
                isLegalMove = false;
            else
                return false;

            // Checking open spaces between destination and the Rook
            if (oldRow < newRow && oldColumn == newColumn)
            {
                distance = newRow - oldRow - 1;
                while (distance > 0)
                {
                    if (cells[oldRow + distance, oldColumn].getSoldier() != null)
                        return false;
                    distance--;
                }
                isLegalMove = true;
            }
            if (oldRow > newRow && oldColumn == newColumn)
            {
                distance = oldRow - newRow - 1;
                while (distance > 0)
                {
                    if (cells[oldRow - distance, oldColumn].getSoldier() != null)
                        return false;
                    distance--;
                }
                isLegalMove = true;
            }
            if (oldRow == newRow && oldColumn < newColumn)
            {
                distance = newColumn - oldColumn - 1;
                while (distance > 0)
                {
                    if (cells[oldRow, oldColumn + distance].getSoldier() != null)
                        return false;
                    distance--;
                }
                isLegalMove = true;
            }
            if (oldRow == newRow && oldColumn > newColumn)
            {
                distance = oldColumn - newColumn - 1;
                while (distance > 0)
                {
                    if (cells[oldRow, oldColumn - distance].getSoldier() != null)
                        return false;
                    distance--;
                }
                isLegalMove = true;
            }
            if (isLegalMove)
                isMoved = true;
            return isLegalMove;

        }
        public bool getIsMoved()
        {
            return isMoved;
        }
        public void setIsMoved()
        {
            this.isMoved = true;
        }
        public void setNotMoved()
        {
            this.isMoved = false;
        }
        public override bool setMovePossible(Cell[,] cells,King king)
        {
            int row = this.getRow();
            int column = this.getColumn();
            Rook other = (Rook)cells[row, column].getSoldier();

            cells[row, column].setSoldier(null);
            if (!king.setIsInDanger(cells))
            {
                if (row < 7)
                    if (cells[row + 1, column].getSoldier() == null || cells[row + 1, column].getSoldier().getIsWhite() != this.getIsWhite())
                    {
                        cells[row, column].setSoldier(other);
                        return true;
                    }

                if (row > 0)
                    if (cells[row - 1, column].getSoldier() == null || cells[row - 1, column].getSoldier().getIsWhite() != this.getIsWhite())
                    {
                        cells[row, column].setSoldier(other);
                        return true;
                    }

                if (column < 7)
                    if (cells[row, column + 1].getSoldier() == null || cells[row, column + 1].getSoldier().getIsWhite() != this.getIsWhite())
                    {
                        cells[row, column].setSoldier(other);
                        return true;
                    }

                if (column > 0)
                    if (cells[row, column - 1].getSoldier() == null || cells[row, column - 1].getSoldier().getIsWhite() != this.getIsWhite())
                    {
                        cells[row, column].setSoldier(other);
                        return true;
                    }
            }
            cells[row, column].setSoldier(other);
            return false;
        }
    }
    class Knight : ChessPiece
    {
        bool isLegalMove;
        public Knight (string name, bool isWhite) : base(name, isWhite) { }
        public bool legalMove (Cell[,] cells, int oldRow, int oldColumn, int newRow, int newColumn)
        {
            // 8 cases , +1+2 , +1-2, +2+1 , +2-1 , -1+2 , -1-2 , -2+1 , -2-1

            isLegalMove = oldRow - newRow == 1 && oldColumn - newColumn == 2 ? true : false;
            if (isLegalMove)
                if (cells[newRow, newColumn].getSoldier() == null || cells[newRow, newColumn].getSoldier().getIsWhite() != cells[oldRow, oldColumn].getSoldier().getIsWhite())
                    return true;
            
            isLegalMove = oldRow - newRow == 1 && oldColumn - newColumn == -2 ? true : false;
            if (isLegalMove)
                if (cells[newRow, newColumn].getSoldier() == null || cells[newRow, newColumn].getSoldier().getIsWhite() != cells[oldRow, oldColumn].getSoldier().getIsWhite())
                    return true;
            isLegalMove = oldRow - newRow == 2 && oldColumn - newColumn == 1 ? true : false;
            if (isLegalMove)
                if (cells[newRow, newColumn].getSoldier() == null || cells[newRow, newColumn].getSoldier().getIsWhite() != cells[oldRow, oldColumn].getSoldier().getIsWhite())
                    return true;
            isLegalMove = oldRow - newRow == 2 && oldColumn - newColumn == -1 ? true : false;
            if (isLegalMove)
                if (cells[newRow, newColumn].getSoldier() == null || cells[newRow, newColumn].getSoldier().getIsWhite() != cells[oldRow, oldColumn].getSoldier().getIsWhite())
                    return true;
            isLegalMove = oldRow - newRow == -1 && oldColumn - newColumn == 2 ? true : false;
            if (isLegalMove)
                if (cells[newRow, newColumn].getSoldier() == null || cells[newRow, newColumn].getSoldier().getIsWhite() != cells[oldRow, oldColumn].getSoldier().getIsWhite())
                    return true;
            isLegalMove = oldRow - newRow == -1 && oldColumn - newColumn == -2 ? true : false;
            if (isLegalMove)
                if (cells[newRow, newColumn].getSoldier() == null || cells[newRow, newColumn].getSoldier().getIsWhite() != cells[oldRow, oldColumn].getSoldier().getIsWhite())
                    return true;
            isLegalMove = oldRow - newRow == -2 && oldColumn - newColumn == 1 ? true : false;
            if (isLegalMove)
                if (cells[newRow, newColumn].getSoldier() == null || cells[newRow, newColumn].getSoldier().getIsWhite() != cells[oldRow, oldColumn].getSoldier().getIsWhite())
                    return true;
            isLegalMove = oldRow - newRow == -2 && oldColumn - newColumn == -1 ? true : false;
            if (isLegalMove)
                if (cells[newRow, newColumn].getSoldier() == null || cells[newRow, newColumn].getSoldier().getIsWhite() != cells[oldRow, oldColumn].getSoldier().getIsWhite())
                    return true;
            return isLegalMove;
        }
        public override bool setMovePossible(Cell[,] cells, King king)
        {
            int row = this.getRow();
            int column = this.getColumn();
            Knight other = (Knight)cells[row, column].getSoldier();

            cells[row, column].setSoldier(null);
            if (!king.setIsInDanger(cells))
            {
                if (row + 1 <= 7 && column + 2 <= 7)
                    if (cells[row + 1, column + 2].getSoldier() == null || cells[row + 1, column + 2].getSoldier().getIsWhite() != this.getIsWhite())
                    {
                        cells[row, column].setSoldier(other);
                        return true;
                    }

                if (row + 1 <= 7 && column - 2 >= 0)
                    if (cells[row + 1, column - 2].getSoldier() == null || cells[row + 1, column - 2].getSoldier().getIsWhite() != this.getIsWhite())
                    {
                        cells[row, column].setSoldier(other);
                        return true;
                    }

                if (row + 2 <= 7 && column + 1 <= 7)
                    if (cells[row + 2, column + 1].getSoldier() == null || cells[row + 2, column + 1].getSoldier().getIsWhite() != this.getIsWhite())
                    {
                        cells[row, column].setSoldier(other);
                        return true;
                    }

                if (row + 2 <= 7 && column - 1 >= 0)
                    if (cells[row + 2, column - 1].getSoldier() == null || cells[row + 2, column - 1].getSoldier().getIsWhite() != this.getIsWhite())
                    {
                        cells[row, column].setSoldier(other);
                        return true;
                    }

                if (row - 1 >= 0 && column + 2 <= 7)
                    if (cells[row - 1, column + 2].getSoldier() == null || cells[row - 1, column + 2].getSoldier().getIsWhite() != this.getIsWhite())
                    {
                        cells[row, column].setSoldier(other);
                        return true;
                    }

                if (row - 1 >= 0 && column - 2 >= 0)
                    if (cells[row - 1, column - 2].getSoldier() == null || cells[row - 1, column - 2].getSoldier().getIsWhite() != this.getIsWhite())
                    {
                        cells[row, column].setSoldier(other);
                        return true;
                    }

                if (row - 2 >= 0 && column + 1 <= 7)
                    if (cells[row - 2, column + 1].getSoldier() == null || cells[row - 2, column + 1].getSoldier().getIsWhite() != this.getIsWhite())
                    {
                        cells[row, column].setSoldier(other);
                        return true;
                    }

                if (row - 2 >= 0 && column - 1 >= 0)
                    if (cells[row - 2, column - 1].getSoldier() == null || cells[row - 2, column - 1].getSoldier().getIsWhite() != this.getIsWhite())
                    {
                        cells[row, column].setSoldier(other);
                        return true;
                    }
            }
            cells[row, column].setSoldier(other);
            return false;

        }
    }
    class Bishop : ChessPiece
    {
        bool isLegalMove;
        public Bishop (string name, bool isWhite) : base(name, isWhite) { }
        public virtual bool legalMove(Cell[,] cells, int oldRow, int oldColumn, int newRow, int newColumn)
        {
            int distanceX;
            int distanceY;
            isLegalMove = false;

            distanceX = oldRow > newRow? oldRow - newRow: newRow - oldRow;
            distanceY = oldColumn > newColumn ? oldColumn - newColumn : newColumn - oldColumn;
            if (distanceX != distanceY)
                return false;

            // 4 cases:   +raw + column , + raw - column , -raw +column , -raw -column

            if (oldRow > newRow && oldColumn > newColumn)
            {
                for (int i = 1; i < distanceX && isLegalMove; i++)
                {
                    isLegalMove = cells[oldRow - i, oldColumn - i].getSoldier() == null;
                    if (!isLegalMove)
                        return false;
                }

            }
            if (oldRow < newRow && oldColumn < newColumn)
            {
                for (int i = 1; i < distanceX && isLegalMove; i++)
                {
                    isLegalMove = cells[oldRow + i, oldColumn + i].getSoldier() == null;
                    if (!isLegalMove)
                        return false;
                }

            }
            if (oldRow < newRow && oldColumn > newColumn)
            {
                for (int i = 1; i < distanceX && i < distanceY ; i++)
                {
                    isLegalMove = cells[oldRow + i, oldColumn - i].getSoldier() == null;
                    if (!isLegalMove)
                        return false;
                }

            }
            if (oldRow > newRow && oldColumn < newColumn)
            {
                for (int i = 1; i < distanceX && i < distanceY ; i++)
                {
                    isLegalMove = cells[oldRow - i, oldColumn + i].getSoldier() == null;
                    if (!isLegalMove)
                        return false;
                }
            }
            if (cells[newRow, newColumn].getSoldier() == null)
                return true;
            if (cells[newRow, newColumn].getSoldier().getIsWhite() != cells[oldRow, oldColumn].getSoldier().getIsWhite())
                return true;

            return isLegalMove;
        }
        public override bool setMovePossible(Cell[,] cells, King king)
        {
            int row = this.getRow();
            int column = this.getColumn();
            Bishop other = (Bishop)cells[row, column].getSoldier();

            cells[row, column].setSoldier(null);
            if (!king.setIsInDanger(cells))
            {
                if (row > 0 && column > 0)
                    if (cells[row - 1, column - 1].getSoldier() == null || cells[row - 1, column - 1].getSoldier().getIsWhite() != this.getIsWhite())
                    {
                        cells[row, column].setSoldier(other);
                        return true;
                    }

                if (row > 0 && column < 7)
                    if (cells[row - 1, column + 1].getSoldier() == null || cells[row - 1, column + 1].getSoldier().getIsWhite() != this.getIsWhite())
                    {
                        cells[row, column].setSoldier(other);
                        return true;
                    }

                if (row < 7 && column > 0)
                    if (cells[row + 1, column - 1].getSoldier() == null || cells[row + 1, column - 1].getSoldier().getIsWhite() != this.getIsWhite())
                    {
                        cells[row, column].setSoldier(other);
                        return true;
                    }

                if (row < 7 && column < 7)
                    if (cells[row + 1, column + 1].getSoldier() == null || cells[row + 1, column + 1].getSoldier().getIsWhite() != this.getIsWhite())
                    {
                        cells[row, column].setSoldier(other);
                        return true;
                    }
            }
            cells[row, column].setSoldier(other);
            return false;
        }
    }
    class Queen : Bishop
    {
        bool isLegalMove;
        public Queen (string name, bool isWhite) : base(name, isWhite) { }
        public override bool legalMove(Cell[,] cells, int oldRow, int oldColumn, int newRow, int newColumn)
        {
            // Move Check if it's Bishops move
            if (base.legalMove(cells, oldRow, oldColumn, newRow, newColumn))
                return true;

            int distance;
            isLegalMove = false;

            // check if Destination is Clear or enemy
            if (cells[newRow, newColumn].getSoldier() == null || cells[newRow, newColumn].getSoldier().getIsWhite() != cells[oldRow, oldColumn].getSoldier().getIsWhite())
                isLegalMove = false;
            else
                return false;

            // Checking open spaces between destination and the Rook
            if (oldRow < newRow && oldColumn == newColumn)
            {
                distance = newRow - oldRow - 1;
                while (distance > 0)
                {
                    if (cells[oldRow + distance, oldColumn].getSoldier() != null)
                        return false;
                    distance--;
                }
                isLegalMove = true;
            }
            if (oldRow > newRow && oldColumn == newColumn)
            {
                distance = oldRow - newRow - 1;
                while (distance > 0)
                {
                    if (cells[oldRow - distance, oldColumn].getSoldier() != null)
                        return false;
                    distance--;
                }
                isLegalMove = true;
            }
            if (oldRow == newRow && oldColumn < newColumn)
            {
                distance = newColumn - oldColumn - 1;
                while (distance > 0)
                {
                    if (cells[oldRow, oldColumn + distance].getSoldier() != null)
                        return false;
                    distance--;
                }
                isLegalMove = true;
            }
            if (oldRow == newRow && oldColumn > newColumn)
            {
                distance = oldColumn - newColumn - 1;
                while (distance > 0)
                {
                    if (cells[oldRow, oldColumn - distance].getSoldier() != null)
                        return false;
                    distance--;
                }
                isLegalMove = true;
            }
            return isLegalMove;
        }
        public override bool setMovePossible(Cell[,] cells, King king)
        {
            if (base.setMovePossible(cells, king))
                return true;

            int row = this.getRow();
            int column = this.getColumn();
            Queen other = (Queen)cells[row, column].getSoldier();

            cells[row, column].setSoldier(null);
            if (!king.setIsInDanger(cells))
            {
                if (row < 7)
                    if (cells[row + 1, column].getSoldier() == null || cells[row + 1, column].getSoldier().getIsWhite() != this.getIsWhite())
                    {
                        cells[row, column].setSoldier(other);
                        return true;
                    }

                if (row > 0)
                    if (cells[row - 1, column].getSoldier() == null || cells[row - 1, column].getSoldier().getIsWhite() != this.getIsWhite())
                    {
                        cells[row, column].setSoldier(other);
                        return true;
                    }

                if (column < 7)
                    if (cells[row, column + 1].getSoldier() == null || cells[row, column + 1].getSoldier().getIsWhite() != this.getIsWhite())
                    {
                        cells[row, column].setSoldier(other);
                        return true;
                    }

                if (column > 0)
                    if (cells[row, column - 1].getSoldier() == null || cells[row, column - 1].getSoldier().getIsWhite() != this.getIsWhite())
                    {
                        cells[row, column].setSoldier(other);
                        return true;
                    }
            }

            cells[row, column].setSoldier(other);
            return false;
        }
    }
    class King : ChessPiece
    {
        bool isLegalMove;
        bool isMoved;
        bool isInDanger;
        string whereAttackComeFrom;
        public override void setSoldier(ChessPiece soldier)
        {
            base.setSoldier(soldier);
        }
        public override ChessPiece getSoldier()
        {
            return base.getSoldier();
        }
        public King (string name, bool isWhite) : base(name, isWhite)
        {
            if (isWhite)
                setRow(0);
            else
                setRow(7);
            setColumn(4);
            isInDanger = false;
            
        }
        public bool legalMove(Cell[,] cells, int oldRow, int oldColumn, int newRow, int newColumn)
        {
            if (oldRow == newRow && oldColumn == newColumn)
                return false;

            // all moves possible: 

            int checkIfLegal = 0;
            checkIfLegal = oldRow == newRow && oldColumn == newColumn + 1 ? checkIfLegal + 1 : checkIfLegal;
            checkIfLegal = oldRow == newRow && oldColumn == newColumn - 1 ? checkIfLegal + 1 : checkIfLegal;
            checkIfLegal = oldRow == newRow +1 && oldColumn == newColumn ? checkIfLegal + 1 : checkIfLegal;
            checkIfLegal = oldRow == newRow -1 && oldColumn == newColumn ? checkIfLegal + 1 : checkIfLegal;
            checkIfLegal = oldRow == newRow +1 && oldColumn == newColumn + 1 ? checkIfLegal + 1 : checkIfLegal;
            checkIfLegal = oldRow == newRow +1 && oldColumn == newColumn - 1 ? checkIfLegal + 1 : checkIfLegal;
            checkIfLegal = oldRow == newRow -1 && oldColumn == newColumn +1 ? checkIfLegal + 1: checkIfLegal;
            checkIfLegal = oldRow == newRow -1 && oldColumn == newColumn -1 ? checkIfLegal + 1 : checkIfLegal;


            // Checking Casteling 
            if (newColumn == (oldColumn + 2) || newColumn == (oldColumn - 2))
            {

                bool isChecked = false;
                if (isMoved)
                    return false;

                switch (cells[oldRow, oldColumn].getSoldier().getIsWhite())
                {
                    case true:
                        // White King Cateling
                        if (newColumn == oldColumn + 2)
                        {
                            // check if currently Checked
                            King king = (King)this;
                            if (isChecked = king.setIsInDanger(cells))
                            {
                                Console.WriteLine("King is Checked cannot Castle");
                                return false;
                            }
                            int distance = 1;
                            while (oldColumn + distance < 7)
                            {
                                if (cells[oldRow, oldColumn + distance].getSoldier() != null)
                                    return false;
                                distance++;
                            }
                            
                            // check if next tile is Checked
                            king.setColumn(oldColumn + 1);
                            cells[oldRow, oldColumn + 1].setSoldier(king);
                            if (isChecked = king.setIsInDanger(cells))
                            {
                                cells[oldRow, oldColumn + 1].setSoldier(null);
                                king.isInDanger = false;
                                king.setColumn(4);
                                return false;
                            }
                            cells[oldRow, oldColumn + 1].setSoldier(null);

                            // check if destination is checked
                            king.setColumn(oldColumn + 2);
                            cells[oldRow, oldColumn + 2].setSoldier(king);
                            if (isChecked = king.setIsInDanger(cells))
                            {
                                cells[oldRow, oldColumn + 2].setSoldier(null);
                                king.isInDanger = false;
                                king.setColumn(4);
                                return false;
                            }
                            cells[oldRow, oldColumn + 2].setSoldier(null);

                            if (cells[0, 7].getSoldier() is Rook)
                            {
                                Rook other = (Rook)cells[0, 7].getSoldier();
                                if (other.getIsMoved())
                                    return false;

                                    other.setIsMoved();
                                    isMoved = true;
                                    cells[0, 5].setSoldier(other);
                                    cells[0, 7].setSoldier(null);
                                    return true;
                            }

                        }
                        if (newColumn == oldColumn - 2)
                        {
                            // check if currently Checked
                            King king = (King)this;
                            if (isChecked = king.setIsInDanger(cells))
                            {
                                Console.WriteLine("King is Checked cannot Castle");
                                return false;
                            }
                            int distance = 1;
                            while (oldColumn - distance > 0)
                            {
                                if (cells[oldRow, oldColumn - distance].getSoldier() != null)
                                    return false;
                                distance++;
                            }

                            // check if next tile is Checked
                            king.setColumn(oldColumn - 1);
                            cells[oldRow, oldColumn - 1].setSoldier(king);
                            if (isChecked = king.setIsInDanger(cells))
                            {
                                cells[oldRow, oldColumn - 1].setSoldier(null);
                                king.isInDanger = false;
                                king.setColumn(4);
                                return false;
                            }
                            cells[oldRow, oldColumn - 1].setSoldier(null);

                            // check if destination is checked
                            king.setColumn(oldColumn - 2);
                            cells[oldRow, oldColumn - 2].setSoldier(king);
                            if (isChecked = king.setIsInDanger(cells))
                            {
                                cells[oldRow, oldColumn - 2].setSoldier(null);
                                king.isInDanger = false;
                                king.setColumn(4);
                                return false;
                            }
                            cells[oldRow, oldColumn - 2].setSoldier(null);

                            if (cells[0, 0].getSoldier() is Rook)
                            {
                                Rook other = (Rook)cells[0, 0].getSoldier();
                                if (other.getIsMoved())
                                    return false;

                                    other.setIsMoved();
                                    isMoved = true;
                                    cells[0, 3].setSoldier(other);
                                    cells[0, 0].setSoldier(null);
                                    return true;
                            }
                        }
                        break;

                    case false:
                        //Black king casteling
                        if (newColumn == oldColumn + 2)
                        {
                            King king = (King)this;
                            if (isChecked = king.setIsInDanger(cells))
                            {
                                Console.WriteLine("King is Checked cannot Castle");
                                return false;
                            }
                            int distance = 1;
                            while (oldColumn + distance < 7)
                            {
                                if (cells[oldRow, oldColumn + distance].getSoldier() != null)
                                    return false;
                                distance++;
                            }
                            // check if next tile is Checked
                            king.setColumn(oldColumn + 1);
                            cells[oldRow, oldColumn + 1].setSoldier(king);
                            if (isChecked = king.setIsInDanger(cells))
                            {
                                cells[oldRow, oldColumn + 1].setSoldier(null);
                                king.isInDanger = false;
                                king.setColumn(4);
                                return false;
                            }
                            cells[oldRow, oldColumn + 1].setSoldier(null);


                            // check if destination is checked
                            king.setColumn(oldColumn + 2);
                            cells[oldRow, oldColumn + 2].setSoldier(king);
                            if (isChecked = king.setIsInDanger(cells))
                            {
                                cells[oldRow, oldColumn + 2].setSoldier(null);
                                king.isInDanger = false;
                                king.setColumn(4);
                                return false;
                            }
                            cells[oldRow, oldColumn + 2].setSoldier(null);

                            if (cells[7, 7].getSoldier() is Rook)
                            {
                                Rook other = (Rook)cells[7, 7].getSoldier();
                                if (other.getIsMoved())
                                    return false;

                                    other.setIsMoved();
                                    isMoved = true;
                                    cells[7, 5].setSoldier(other);
                                    cells[7, 7].setSoldier(null);
                                    setRow(newRow);
                                    setColumn(newColumn);
                                    return true;
                            }
                        }
                        if (newColumn == oldColumn - 2)
                        {
                            King king = (King)this;
                            if (isChecked = king.setIsInDanger(cells))
                            {
                                Console.WriteLine("King is Checked cannot Castle");
                                return false;
                            }
                            int distance = 1;
                            while (oldColumn - distance > 0)
                            {
                                if (cells[oldRow, oldColumn - distance].getSoldier() != null)
                                    return false;
                                distance++;
                            }

                            // check if next tile is Checked
                            king.setColumn(oldColumn - 1);
                            cells[oldRow, oldColumn - 1].setSoldier(king);
                            if (isChecked = king.setIsInDanger(cells))
                            {
                                cells[oldRow, oldColumn - 1].setSoldier(null);
                                king.isInDanger = false;
                                king.setColumn(4);
                                return false;
                            }
                            cells[oldRow, oldColumn - 1].setSoldier(null);

                            // check if destination is checked
                            king.setColumn(oldColumn - 2);
                            cells[oldRow, oldColumn - 2].setSoldier(king);
                            if (isChecked = king.setIsInDanger(cells))
                            {
                                cells[oldRow, oldColumn - 2].setSoldier(null);
                                king.isInDanger = false;
                                king.setColumn(4);
                                return false;
                            }
                            cells[oldRow, oldColumn - 2].setSoldier(null);

                            if (cells[7, 0].getSoldier() is Rook)
                            {
                                Rook other = (Rook)cells[7, 0].getSoldier();
                                if (other.getIsMoved())
                                    return false;

                                    other.setIsMoved();
                                    isMoved = true;
                                    cells[7, 3].setSoldier(other);
                                    cells[7, 0].setSoldier(null);
                                    setRow(newRow);
                                    setColumn(newColumn);
                                    return true;                          
                            }
                        }

                        break;
                }

            }
            if (checkIfLegal == 0)
                return false;

            if (cells[newRow, newColumn].getSoldier() == null || cells[newRow, newColumn].getSoldier().getIsWhite() != cells[oldRow, oldColumn].getSoldier().getIsWhite())
            {
                isMoved = true;
                isLegalMove = true;
                setRow(newRow);
                setColumn(newColumn);
            }
            return isLegalMove;
        }
        public bool getIsMoved()
        {
            return isMoved;
        }
        public bool setIsInDanger(Cell[,] cells)
        {
            int row = this.getRow();
            int column = this.getColumn();
            bool blackRound1;
            bool whiteRound1;
            isInDanger = false;
            if (this.getIsWhite() == true)
            {
                blackRound1 = false;
                whiteRound1 = true;
            }
            else
            {
                blackRound1 = true;
                whiteRound1 = false;
            }
            
            //checking for knights
            if (row + 1 <= 7 && column + 2 <= 7)
                if (cells[row + 1, column + 2].getSoldier() is Knight && cells[row + 1, column + 2].getSoldier().getIsWhite() == blackRound1)
                {
                    whereAttackComeFrom = (row + 1) + "" + (column + 2);
                    return isInDanger = true;                   
                }

            if (row + 1 <= 7 && column - 2 >= 0)
                if (cells[row + 1, column - 2].getSoldier() is Knight && cells[row + 1, column - 2].getSoldier().getIsWhite() == blackRound1)
                {
                    whereAttackComeFrom = (row + 1) + "" + (column - 2);
                    return isInDanger = true;
                }
            if (row - 1 >= 0 && column + 2 <= 7)
                if (cells[row - 1, column + 2].getSoldier() is Knight && cells[row - 1, column + 2].getSoldier().getIsWhite() == blackRound1)
                {
                    whereAttackComeFrom = (row - 1) + "" + (column + 2);
                    return isInDanger = true;
                }
            if (row - 1 >= 0 && column - 2 >= 0)
                if (cells[row - 1, column - 2].getSoldier() is Knight && cells[row - 1, column - 2].getSoldier().getIsWhite() == blackRound1)
                {
                    whereAttackComeFrom = (row - 1) + "" + (column - 2);
                    return isInDanger = true;
                }
            if (row + 2 <= 7 && column + 1 <= 7)
                if (cells[row + 2, column + 1].getSoldier() is Knight && cells[row + 2, column + 1].getSoldier().getIsWhite() == blackRound1)
                {
                    whereAttackComeFrom = (row + 2) + "" + (column + 1);
                    return isInDanger = true;
                }
            if (row + 2 <= 7 && column - 1 >= 0)
                if (cells[row + 2, column - 1].getSoldier() is Knight && cells[row + 2, column - 1].getSoldier().getIsWhite() == blackRound1)
                {
                    whereAttackComeFrom = (row + 2) + "" + (column - 1);
                    return isInDanger = true;
                }
            if (row - 2 >= 0 && column + 1 <= 7)
                if (cells[row - 2, column + 1].getSoldier() is Knight && cells[row - 2, column + 1].getSoldier().getIsWhite() == blackRound1)
                {
                    whereAttackComeFrom = (row - 2) + "" + (column + 1);
                    return isInDanger = true;
                }
            if (row - 2 >= 0 && column - 1 >= 0)
                if (cells[row - 2, column - 1].getSoldier() is Knight && cells[row - 2, column - 1].getSoldier().getIsWhite() == blackRound1)
                {
                    whereAttackComeFrom = (row - 2) + "" + (column - 1);
                    return isInDanger = true;
                }

            // checking for Bishop Queen and Pawns
            // case R+ C+
            for (int i = 1; i + row <= 7 && i + column <= 7; i++)
            {
                if (cells[row + i, column + i].getSoldier() != null && cells[row + i, column + i].getSoldier().getIsWhite() == whiteRound1)
                    break;
                if (cells[row + i, column + i].getSoldier() != null && cells[row + i, column + i].getSoldier().getIsWhite() == blackRound1)
                {
                    if (cells[row + i, column + i].getSoldier() is Bishop)
                    {
                        whereAttackComeFrom = (row + i) + "" + (column + i);
                        return isInDanger = true;
                    }
                    if (cells[row + i, column + i].getSoldier() is Pawn && i == 1)
                    {
                        whereAttackComeFrom = (row + i) + "" + (column + i);
                        return isInDanger = true;
                    }
                    if (cells[row + i, column + i].getSoldier() is King && i == 1)
                    {
                        whereAttackComeFrom = (row + i) + "" + (column + i);
                        return isInDanger = true;
                    }
                }
            }
            // case R- C-
            for (int i = 1; row - i >= 0 && column - i >= 0; i++)
            {
                if (cells[row - i, column - i].getSoldier() != null && cells[row - i, column - i].getSoldier().getIsWhite() == whiteRound1)
                    break;
                if (cells[row - i, column - i].getSoldier() != null && cells[row - i, column - i].getSoldier().getIsWhite() == blackRound1)
                {
                    if (cells[row - i, column - i].getSoldier() is Bishop)
                    {
                        whereAttackComeFrom = (row - i) + "" + (column - i);                       
                        return isInDanger = true;
                    }
                    if (cells[row - i, column - i].getSoldier() is Pawn && i == 1)
                    {
                        whereAttackComeFrom = (row - i) + "" + (column - i);
                        return isInDanger = true;
                    }
                    if (cells[row - i, column - i].getSoldier() is King && i == 1)
                    {
                        whereAttackComeFrom = (row - i) + "" + (column - i);
                        return isInDanger = true;
                    }
                }

            }
            // case R+ C-
            for (int i = 1, j = 1; row + i <= 7 && column - j >= 0; i++, j++)
            {

                if (cells[row + i, column - j].getSoldier() != null && cells[row + i, column - j].getSoldier().getIsWhite() == whiteRound1)
                    break;
                if (cells[row + i, column - j].getSoldier() != null && cells[row + i, column - j].getSoldier().getIsWhite() == blackRound1)
                {
                    if (cells[row + i, column - j].getSoldier() is Bishop)
                    {
                        whereAttackComeFrom = (row + i) + "" + (column - j);
                        return isInDanger = true;
                    }
                    if (cells[row + i, column - j].getSoldier() is Pawn && i == 1)
                    {
                        whereAttackComeFrom = (row + i) + "" + (column - j);
                        return isInDanger = true;
                    }
                    if (cells[row + i, column - j].getSoldier() is King && i == 1)
                    {
                        whereAttackComeFrom = (row + i) + "" + (column - j);
                        return isInDanger = true;
                    }
                }
            }
            // case R- C+
            for (int i = 1, j = 1; row - i >= 0 && column + j <= 7; i++, j++)
            {
                if (cells[row - i, column + j].getSoldier() != null && cells[row - i, column + j].getSoldier().getIsWhite() == whiteRound1)
                    break;
                if (cells[row - i, column + j].getSoldier() != null && cells[row - i, column + j].getSoldier().getIsWhite() == blackRound1)
                {
                    if (cells[row - i, column + j].getSoldier() is Bishop)
                    {
                        whereAttackComeFrom = (row - i) + "" + (column + j);
                        return  isInDanger = true;
                    }
                    if (cells[row - i, column + j].getSoldier() is Pawn && i == 1)
                    {
                        whereAttackComeFrom = (row - i) + "" + (column + j);
                        return isInDanger = true;                       
                    }
                    if (cells[row - i, column + j].getSoldier() is King && i == 1)
                    {
                        whereAttackComeFrom = (row - i) + "" + (column + j);
                        return isInDanger = true;
                    }
                }
            }

            // checking for Queen and Rooks
            //case R+ 
            for (int i = 1; row + i <= 7; i++)
            {
                if (cells[row + i, column].getSoldier() != null && cells[row + i, column].getSoldier().getIsWhite() == whiteRound1)
                    break;
                if (cells[row + i, column].getSoldier() != null && cells[row + i, column].getSoldier().getIsWhite() == blackRound1)
                {
                    if (cells[row + i, column].getSoldier() is Rook || cells[row + i, column].getSoldier() is Queen)
                    {
                        whereAttackComeFrom = (row + i) + "" + (column);                      
                        return isInDanger = true;
                    }
                    if (cells[row + i, column].getSoldier() is King && i == 1)
                    {
                        whereAttackComeFrom = (row + i) + "" + column;
                        return isInDanger = true;
                    }
                }
            }
            //case R-
            for (int i = 1; row - i >= 0; i++)
            {
                if (cells[row - i, column].getSoldier() != null && cells[row - i, column].getSoldier().getIsWhite() == whiteRound1)
                    break;
                if (cells[row - i, column].getSoldier() != null && cells[row - i, column].getSoldier().getIsWhite() == blackRound1)
                {
                    if (cells[row - i, column].getSoldier() is Rook || cells[row - i, column].getSoldier() is Queen)
                    {
                        whereAttackComeFrom = (row - i) + "" + (column);
                        return isInDanger = true;                         
                    }
                    if (cells[row - i, column].getSoldier() is King && i == 1)
                    {
                        whereAttackComeFrom = (row - i) + "" + column;
                        return isInDanger = true;
                    }
                }
            }
            //case C-
            for (int i = 1; column - i >= 0; i++)
            {
                if (cells[row, column - i].getSoldier() != null && cells[row, column - i].getSoldier().getIsWhite() == whiteRound1)
                    break;
                if (cells[row, column - i].getSoldier() != null && cells[row, column - i].getSoldier().getIsWhite() == blackRound1)
                {
                    if (cells[row, column - i].getSoldier() is Rook || cells[row, column - i].getSoldier() is Queen)
                    {
                        whereAttackComeFrom = (row) + "" + (column - i);
                        return isInDanger = true;                       
                    }
                    if (cells[row, column - 1].getSoldier() is King && i == 1)
                    {
                        whereAttackComeFrom = row + "" + (column - i);
                        return isInDanger = true;
                    }
                }
            }
            //case C+
            for (int i = 1; column + i <= 7; i++)
            {
                if (cells[row, column + i].getSoldier() != null && cells[row, column + i].getSoldier().getIsWhite() == whiteRound1)
                    break;
                if (cells[row, column + i].getSoldier() != null && cells[row, column + i].getSoldier().getIsWhite() == blackRound1)
                {
                    if (cells[row, column + i].getSoldier() is Rook || cells[row, column + i].getSoldier() is Queen)
                    {
                        whereAttackComeFrom = (row) + "" + (column + i);
                        return isInDanger = true;            
                    }
                    if (cells[row, column + 1].getSoldier() is King && i == 1)
                    {
                        whereAttackComeFrom = row + "" + (column + i);
                        return isInDanger = true;
                    }
                }
            }
            whereAttackComeFrom = "";
            return isInDanger;
        }
        public bool canEscape(Cell[,] board)
        {
            bool canEscape = false;
            int row = this.getRow();
            int column = this.getColumn();
            King currentKing = (King)board[row, column].getSoldier();
            ChessPiece other;

            if (row + 1 <= 7)
            {
                if(board[row + 1, column].getSoldier() == null)
                {
                    currentKing.setRow(row + 1);
                    board[row + 1, column].setSoldier(currentKing);
                    canEscape = !currentKing.setIsInDanger(board);
                    board[row + 1, column].setSoldier(null);
                    currentKing.setRow(row);
                    currentKing.setColumn(column);
                    if (canEscape)
                        return true;
                }
                else if (board[row + 1, column].getSoldier().getIsWhite() != currentKing.getIsWhite())
                {
                    other = board[row + 1, column].getSoldier();
                    currentKing.setRow(row + 1);
                    board[row + 1, column].setSoldier(currentKing);
                    canEscape = !currentKing.setIsInDanger(board);
                    board[row + 1, column].setSoldier(null);
                    currentKing.setRow(row);
                    currentKing.setColumn(column);
                    board[row + 1, column].setSoldier(other);
                    if (canEscape)
                        return true;
                }
            }
            if (row + 1 <= 7 && column - 1 >= 0)
            {
                if (board[row + 1, column - 1].getSoldier() == null)
                {           
                    currentKing.setRow(row + 1);
                    currentKing.setColumn(column - 1);
                    board[row + 1, column - 1].setSoldier(currentKing);
                    canEscape = !currentKing.setIsInDanger(board);
                    board[row + 1, column - 1].setSoldier(null);
                    currentKing.setRow(row);
                    currentKing.setColumn(column);
                    if (canEscape)
                        return true;
                }
                else if(board[row + 1, column - 1].getSoldier().getIsWhite() != currentKing.getIsWhite())
                {
                    other = board[row + 1, column - 1].getSoldier();
                    currentKing.setRow(row + 1);
                    currentKing.setColumn(column - 1);
                    board[row + 1, column - 1].setSoldier(currentKing);
                    canEscape = !currentKing.setIsInDanger(board);
                    board[row + 1, column - 1].setSoldier(null);
                    currentKing.setRow(row);
                    currentKing.setColumn(column);
                    board[row + 1, column - 1].setSoldier(other);
                    if (canEscape)
                        return true;
                }
            }
            if (row + 1 <= 7 && column + 1 <= 7)
            {
                if(board[row + 1, column + 1].getSoldier() == null)
                {
                    currentKing.setRow(row + 1);
                    currentKing.setColumn(column + 1);
                    board[row + 1, column + 1].setSoldier(currentKing);
                    canEscape = !currentKing.setIsInDanger(board);
                    board[row + 1, column + 1].setSoldier(null);
                    currentKing.setRow(row);
                    currentKing.setColumn(column);
                    if (canEscape)
                        return true;
                }
                else if (board[row + 1, column + 1].getSoldier().getIsWhite() != currentKing.getIsWhite())
                {
                    other = board[row + 1, column + 1].getSoldier();
                    currentKing.setRow(row + 1);
                    currentKing.setColumn(column + 1);
                    board[row + 1, column + 1].setSoldier(currentKing);
                    canEscape = !currentKing.setIsInDanger(board);
                    board[row + 1, column + 1].setSoldier(null);
                    currentKing.setRow(row);
                    currentKing.setColumn(column);
                    board[row + 1, column + 1].setSoldier(other);
                    if (canEscape)
                        return true;
                }
            }
            if (row - 1 >= 0)
            {
                if(board[row - 1, column].getSoldier() == null)
                {
                    currentKing.setRow(row - 1);
                    board[row - 1, column].setSoldier(currentKing);
                    canEscape = !currentKing.setIsInDanger(board);
                    board[row - 1, column].setSoldier(null);
                    currentKing.setRow(row);
                    currentKing.setColumn(column);
                    if (canEscape)
                        return true;
                }
                else if(board[row - 1, column].getSoldier().getIsWhite() != currentKing.getIsWhite())
                {
                    other = board[row - 1, column].getSoldier();
                    currentKing.setRow(row - 1);
                    board[row - 1, column].setSoldier(currentKing);
                    canEscape = !currentKing.setIsInDanger(board);
                    board[row - 1, column].setSoldier(null);
                    currentKing.setRow(row);
                    currentKing.setColumn(column);
                    board[row - 1, column].setSoldier(other);
                    if (canEscape)
                        return true;
                }
            }
            if (row - 1 >= 0 && column - 1 >= 0)
            {
                if(board[row - 1, column - 1].getSoldier() == null)
                {
                    currentKing.setRow(row - 1);
                    currentKing.setColumn(column - 1);
                    board[row - 1, column - 1].setSoldier(currentKing);
                    canEscape = !currentKing.setIsInDanger(board);
                    board[row - 1, column - 1].setSoldier(null);
                    currentKing.setRow(row);
                    currentKing.setColumn(column);
                    if (canEscape)
                        return true;
                }
                else if (board[row - 1, column - 1].getSoldier().getIsWhite() != currentKing.getIsWhite())
                {
                    other = board[row - 1, column - 1].getSoldier();
                    currentKing.setRow(row - 1);
                    currentKing.setColumn(column - 1);
                    board[row - 1, column - 1].setSoldier(currentKing);
                    canEscape = !currentKing.setIsInDanger(board);
                    board[row - 1, column - 1].setSoldier(null);
                    currentKing.setRow(row);
                    currentKing.setColumn(column);
                    board[row - 1, column - 1].setSoldier(other);
                    if (canEscape)
                        return true;
                }
            }
            if (row - 1 >= 0 && column + 1 <= 7)
            {
                if (board[row - 1, column + 1].getSoldier() == null)
                {
                    currentKing.setRow(row - 1);
                    currentKing.setColumn(column + 1);
                    board[row - 1, column + 1].setSoldier(currentKing);
                    canEscape = !currentKing.setIsInDanger(board);
                    board[row - 1, column + 1].setSoldier(null);
                    currentKing.setRow(row);
                    currentKing.setColumn(column);
                    if (canEscape)
                        return true;
                }
                else if (board[row - 1, column + 1].getSoldier().getIsWhite() != currentKing.getIsWhite())
                {
                    other = board[row - 1, column + 1].getSoldier();
                    currentKing.setRow(row - 1);
                    currentKing.setColumn(column + 1);
                    board[row - 1, column + 1].setSoldier(currentKing);
                    canEscape = !currentKing.setIsInDanger(board);
                    board[row - 1, column + 1].setSoldier(null);
                    currentKing.setRow(row);
                    currentKing.setColumn(column);
                    board[row - 1, column + 1].setSoldier(other);
                    if (canEscape)
                        return true;
                }
            }
            if (column + 1 <= 7)
            {
                if (board[row, column + 1].getSoldier() == null)
                {
                    currentKing.setColumn(column + 1);
                    board[row, column + 1].setSoldier(currentKing);
                    canEscape = !currentKing.setIsInDanger(board);
                    board[row, column + 1].setSoldier(null);
                    currentKing.setRow(row);
                    currentKing.setColumn(column);
                    if (canEscape)
                        return true;
                }
                else if (board[row, column + 1].getSoldier().getIsWhite() != currentKing.getIsWhite())
                {
                    other = board[row, column + 1].getSoldier();
                    currentKing.setColumn(column + 1);
                    board[row, column + 1].setSoldier(currentKing);
                    canEscape = !currentKing.setIsInDanger(board);
                    board[row, column + 1].setSoldier(null);
                    currentKing.setRow(row);
                    currentKing.setColumn(column);
                    board[row, column + 1].setSoldier(other);
                    if (canEscape)
                        return true;
                }
            }

            if (column - 1 >= 0)
            {
                if (board[row, column - 1].getSoldier() == null)
                {
                    currentKing.setColumn(column - 1);
                    board[row, column - 1].setSoldier(currentKing);
                    canEscape = !currentKing.setIsInDanger(board);
                    board[row, column - 1].setSoldier(null);
                    currentKing.setRow(row);
                    currentKing.setColumn(column);
                    if (canEscape)
                        return true;
                }
                else if (board[row, column - 1].getSoldier().getIsWhite() != currentKing.getIsWhite())
                {
                    other = board[row, column - 1].getSoldier();
                    currentKing.setColumn(column - 1);
                    board[row, column - 1].setSoldier(currentKing);
                    canEscape = !currentKing.setIsInDanger(board);
                    board[row, column - 1].setSoldier(null);
                    currentKing.setRow(row);
                    currentKing.setColumn(column);
                    board[row, column - 1].setSoldier(other);
                    if (canEscape)
                        return true;
                }
            }
        return canEscape;

        }
        public string getwhereAttackComeFrom()
        {
            return whereAttackComeFrom;
        }
        public bool getIsInDanger()
        {
            return isInDanger;
        }
        public bool isBlockPossible(Cell[,] board)
        {
            this.setIsInDanger(board);
            bool canBlock = false;
            string x = "" + whereAttackComeFrom[0];
            string y = "" + whereAttackComeFrom[1];
            int row = int.Parse(x);
            int column = int.Parse(y);
            int distance;
            int rowBlock = this.getRow();
            int columnBlock = this.getColumn();

            //Check if threat can be eaten
            if (this.checkTileForBlock(row, column, board))
                return true;

            // 8 conditions of block
            if (row > this.getRow() && column > this.getColumn())
            {
                distance = row - this.getRow() - 1;
                for (int i = 1; i <= distance; i++)
                {
                    rowBlock = this.getRow() + i;
                    columnBlock = this.getColumn() + i;
                    if (this.checkTileForBlock(rowBlock, columnBlock, board))
                        return true;
                }
            }
            if (row < this.getRow() && column < this.getColumn())
            {
                distance = this.getRow() - row - 1;
                for (int i = 1; i <= distance; i++)
                {
                    rowBlock = this.getRow() - i;
                    columnBlock = this.getColumn() - i;
                    if (this.checkTileForBlock(rowBlock, columnBlock, board))
                        return true;
                }
            }
            if (row > this.getRow() && column < this.getColumn())
            {
                distance = row - this.getRow() - 1;
                for (int i = 1; i <= distance; i++)
                {
                    rowBlock = this.getRow() + i;
                    columnBlock = this.getColumn() - i;
                    if (this.checkTileForBlock(rowBlock, columnBlock, board))
                        return true;
                }
            }
            if (row < this.getRow() && column > this.getColumn())
            {
                distance = this.getRow() - row - 1;
                for (int i = 1; i <= distance; i++)
                {
                    rowBlock = this.getRow() - i;
                    columnBlock = this.getColumn() + i;
                    if (this.checkTileForBlock(rowBlock, columnBlock, board))
                        return true;
                }
            }
            if (row > this.getRow() && column == this.getColumn())
            {
                distance = row - this.getRow() - 1;
                for (int i = 1; i <= distance; i++)
                {
                    rowBlock = this.getRow() + i;
                    if (this.checkTileForBlock(rowBlock, columnBlock, board))
                        return true;
                }
            }
            if (row < this.getRow() && column == this.getColumn())
            {
                distance = this.getRow() - row - 1;
                for (int i = 1; i <= distance; i++)
                {
                    rowBlock = this.getRow() - i;
                    if (this.checkTileForBlock(rowBlock, columnBlock, board))
                        return true;
                }
            }
            if (row == this.getRow() && column > this.getColumn())
            {
                distance = column - this.getColumn() - 1;
                for (int i = 1; i <= distance; i++)
                {
                    columnBlock = this.getColumn() + i;
                    if (this.checkTileForBlock(rowBlock, columnBlock, board))
                        return true;
                }
            }
            if (row == this.getRow() && column < this.getColumn())
            {
                distance = this.getColumn() - column - 1;
                for (int i = 1; i <= distance; i++)
                {
                    columnBlock = this.getColumn() - i;
                    if (this.checkTileForBlock(rowBlock, columnBlock, board))
                        return true;
                }
            }

            return canBlock;
        }
        public bool checkTileForBlock(int newRow, int newColumn, Cell[,] board)
        {
            for (int oldRow = 0; oldRow < board.GetLength(0); oldRow++)
            {
                for (int oldColumn = 0; oldColumn < board.GetLength(1); oldColumn++)
                {
                    if (board[oldRow, oldColumn].getSoldier() != null && board[oldRow, oldColumn].getSoldier().getIsWhite() == this.getIsWhite())
                    {
                        if (board[oldRow, oldColumn].getSoldier() is Pawn)
                        {
                            int move = 0;
                            Pawn other = (Pawn)board[oldRow, oldColumn].getSoldier();
                            if (other.legalMove(board, oldRow, oldColumn, newRow, newColumn, move))
                                return true;
                        }
                        if (board[oldRow, oldColumn].getSoldier() is Knight)
                        {
                            Knight other = (Knight)board[oldRow, oldColumn].getSoldier();
                            if (other.legalMove(board, oldRow, oldColumn, newRow, newColumn))
                                return true;
                        }
                        if (board[oldRow, oldColumn].getSoldier() is Rook)
                        {
                            Rook other = (Rook)board[oldRow, oldColumn].getSoldier();
                            if (other.legalMove(board, oldRow, oldColumn, newRow, newColumn))
                                return true;
                        }
                        if (board[oldRow, oldColumn].getSoldier() is Bishop)
                        {
                            Bishop other = (Bishop)board[oldRow, oldColumn].getSoldier();
                            if (other.legalMove(board, oldRow, oldColumn, newRow, newColumn))
                                return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
