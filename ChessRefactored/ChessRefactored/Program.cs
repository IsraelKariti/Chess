/* Conclustions:
* 1. For testing it's better to send parameters to a function instead of using staic variables
* 2. Where could i have used Interfaces?
* 3. For every method write MULTIPLE TESTS that check every branch of the method logiv
* 
* 4. LOOP WITH:
*                 MANY "continue"s (to each condition)
*                 OR
*                 if else if else if else if
*/
using System.ComponentModel;
using System.Data;

namespace TalpiotChessGame
{
    public class TestUtils
    {
        public static StringWriter sw;
    }
    public class ChessGameLauncher
    {
        public static bool DEBUG_MODE = false;

        public static bool isTestMode = false;
        public static void Main(string[] args = null)
        {
            if (args != null && args.Length > 0 && args[0] == "test") isTestMode = true;

            ChessGame chess = new ChessGame();
            chess.Init();
            chess.PlayGame();
        }
    }
    class ChessGame
    {
        //public static ChessGame chess;
        public Board board;
        public Army whiteArmy;
        public Army blackArmy;

        public Board copyBoard;
        public Army copyWhiteArmy;
        public Army copyBlackArmy;

        bool isSuccessfulTurn;
        private int FiftyMovesCounter;

        private string[] boardHistory;
        bool isTurnWhite = true;
        public ChessGame()
        {
            //chess = this;

            board = new Board();
            whiteArmy = new Army(true, board);
            blackArmy = new Army(false, board);

            copyBoard = new Board();
            copyWhiteArmy = new Army(true, copyBoard);
            copyBlackArmy = new Army(false, copyBoard);

            boardHistory = new string[0];
        }
        public void Init()
        {
            whiteArmy.Init();
            blackArmy.Init();

            FiftyMovesCounter = 0;
        }

        private void HandlePlayerCantPlay()
        {
            if (ChessGameLauncher.isTestMode) Console.SetOut(TestUtils.sw);

            // the player can't play, so we check if white king is threatened
            Cell currKingCell = isTurnWhite ? whiteArmy.Pieces[4].cell : blackArmy.Pieces[4].cell;
            if (currKingCell.IsPieceOnCurrCellThreatenedByEnemy())
                Console.WriteLine("Winner is: " + (isTurnWhite ? "Black" : "White"));
            else
                Console.WriteLine("It's a Pat - Game finished with a draw");

            return;
        }
        private bool HasMetFiftyMovesRuleCondition()
        {
            if (FiftyMovesCounter == 50)
            {
                if (ChessGameLauncher.isTestMode) Console.SetOut(TestUtils.sw);
                Console.WriteLine("Game is Stagnant for 50 turns - It's a Draw!");
                return true;
            }
            else
                return false;

        }
        private void HandleGameInDeadPosition()
        {
            if (ChessGameLauncher.isTestMode) Console.SetOut(TestUtils.sw);
            Console.WriteLine("Dead position - Game finished with a draw");
        }
        private bool IsEndOfGame(string boardState)
        {
            if (HasMetFiftyMovesRuleCondition())
                return true;

            if(IsDeadPosition())
            {
                HandleGameInDeadPosition();
                return true;
            }

            if (IsStateReapeted3Times(boardState))
            {
                if (ChessGameLauncher.isTestMode) Console.SetOut(TestUtils.sw);
                Console.WriteLine("Three fold repetition - Game finished with a draw");
                return true;
            }
            return false;
        }
        private void HandlePlayerPlayedSuccessfully(string boardState)
        {
            SaveState(boardState);

            // clear EnPassant Priviliges for current player
            Army currArmy = isTurnWhite ? whiteArmy : blackArmy;
            currArmy.ClearEnPassantPriviliges();

            isTurnWhite = !isTurnWhite;
        }
        public void PlayGame()
        {
            bool isGameActive = true;
            Console.SetOut(Console.Out);
            Console.WriteLine(board.ToString());
            while(isGameActive)
            {
                FiftyMovesCounter++;
                isSuccessfulTurn = Turn(isTurnWhite);
                if (!isSuccessfulTurn)
                {
                    HandlePlayerCantPlay();
                    return;
                }
                Console.WriteLine(board.ToString());
                string boardState = CreateState();  
                if (IsEndOfGame(boardState))
                    return;

                HandlePlayerPlayedSuccessfully(boardState);
                
            }
        }
        internal bool CanPlayerMove(Piece[] pieces)
        {
            foreach (Piece piece in pieces)
            {
                if (piece.IsActive != false)
                {
                    Position[] positions = piece.FindAllReachablePositions();
                    foreach(Position pos in positions)
                    {
                        if(pos!=null)
                        {
                            bool canPieceMove = IsStepSafeForKing(piece.cell.Pos.row, piece.cell.Pos.col, pos.row, pos.col);
                            if (canPieceMove)
                                return true;
                        }
                    }
                }
            }
            return false;
        }
        private void HandlePawnReachedEndOfBoard(bool isWhite, Pawn pawn)
        {
            Piece newPiece = null, newCopyPiece = null;

            Console.WriteLine("Choose an upgrade:\n1. Queen\n2. Bishop\n3. Knight\n4. Rook\n");
            string input;
            if (ChessGameLauncher.DEBUG_MODE)
                input = "1";
            else
                input = Console.ReadLine();
            while (input != "1" && input != "2" && input != "3" && input != "4")
            {
                Console.WriteLine("wrong input try again:");
                input = Console.ReadLine().Trim();
            }

            newPiece = CreateNewPiece(input, isWhite, board);
            newCopyPiece = CreateNewPiece(input, isWhite, copyBoard);

            // set new piece in army
            Army currArmy = isWhite ? whiteArmy : blackArmy;
            Army currCopyArmy = isWhite ? copyWhiteArmy : copyBlackArmy;
            currArmy.Pieces[pawn.IndexInArmy] = newPiece;
            currCopyArmy.Pieces[pawn.IndexInArmy] = newCopyPiece;

            // set new piece on board
            newPiece.cell = pawn.cell;
            pawn.cell.piece = newPiece;
        }
        private Piece CreateNewPiece(string input, bool isWhite, Board board)
        {
            switch (input)
            {
                case "1":
                    return new Queen(isWhite, board);
                case "2":
                    return new Bishop(isWhite, board);
                case "3":
                    return new Knight(isWhite, board);
                case "4":
                    return new Rook(isWhite, board);
                default:
                    return null;
            }
        }
        public virtual bool IsStepSafeForKing(int origRow, int origCol, int destRow, int destCol)
        {
            // prepare evaluation army
            copyBoard.CleanBoard();
            copyWhiteArmy.CopyAllPositions(whiteArmy);
            copyBlackArmy.CopyAllPositions(blackArmy);

            // move the piece
            MovePieceInEvaluationMode(origRow, origCol, destRow, destCol);

            // check if enemy is now threatening the my king (do it in evaluation mode)
            // for this i need to create a method that checks if an army is threatening the enemy king
            bool isKingInDanger;
            if (board.Cells[origRow, origCol].piece.IsWhite)
                isKingInDanger = copyBlackArmy.IsThreateningEnemyKing();
            else
                isKingInDanger = copyWhiteArmy.IsThreateningEnemyKing();
            return !isKingInDanger;
        }
        protected virtual void MovePieceInEvaluationMode(int row, int col, int destRow, int destCol)
        {
            Cell[,] cells = copyBoard.Cells;
            // if there is a piece on dest - kill it
            if (cells[destRow, destCol].piece != null)
            {
                cells[destRow, destCol].piece.IsActive = false;
            }
            // connect the dest to the piece
            cells[destRow, destCol].piece = cells[row, col].piece;

            // connet piece to dest cell
            cells[destRow, destCol].piece.cell = cells[destRow, destCol];

            // disconnect the origin cell from the piece
            cells[row, col].piece = null;
        }
        internal protected bool Turn(bool isWhite)
        {
            bool canMove = isWhite ? CanPlayerMove(whiteArmy.Pieces) : CanPlayerMove(blackArmy.Pieces);
            if (!canMove)
                return false;
            string input = GetInputFromUser(isWhite);

            Position turnDest = CalcDestinationPosition(input);
            Piece pieceToMove = GetPieceToMove(input);

            if (pieceToMove is Pawn || board.Cells[turnDest.row,turnDest.col].piece != null)
                ResetFiftyMovesCounter();

            pieceToMove.PlayMove(turnDest.row, turnDest.col);

            // incase the end of the board was reached:
            Pawn pawnToMove = pieceToMove as Pawn;
            if (pawnToMove != null && (turnDest.row == 0 || turnDest.row == 7))
                HandlePawnReachedEndOfBoard(pieceToMove.IsWhite, pawnToMove );

            // update all the threatened cells
            UpdateThreatenedCells();

            return true;
        }

        private Piece GetPieceToMove(string input)
        {
            int stepOrigCol = input[0] - 'A';
            int stepOrigRow = 8 - (input[1] - '0');
            Cell stepOrigCell = board.Cells[stepOrigRow, stepOrigCol];
            return stepOrigCell.piece;
        }

        private Position CalcDestinationPosition(string input)
        {
            int stepDestCol = input[2] - 'A';
            int stepDestRow = 8 - (input[3] - '0');
            return new Position(stepDestRow, stepDestCol);
        }

        private void UpdateThreatenedCells()
        {
            board.ClearAllThreatenedCells();
            whiteArmy.SetAllThreatenedCells();
            blackArmy.SetAllThreatenedCells();
        }
        private static int indexForDebug = 0;
        private static string GetInput()
        {
            if (ChessGameLauncher.DEBUG_MODE)
            {
                var data =  new[]
                {
                    "F2F3",
                    "E7E5",
                    "G2G4",
                    "D8H4",
                };
                //string gameMoves = "f2f4 e7e5 f4e5 a7a5 e5e6 a5a4 e6e7 a4a3 b2a3 a8a3 c1a3 b7b5 c2c4 b5c4 a3c5 c8a6 a2a4 a6b5 c5d4 b5a4 a1a4 c7c5 d4c5 d7d6 c5d6 d8a5 a4c4 a5a6 c4d4 a6d6 d4d3 d6d3 h2h4 g7g5 e2e4 d3d6 f1e2 d6e5 g2g4 e5e4 h1h3 e4g4 h3f3 g4f3 h4h5 f3h5 d2d4 g5g4 d4d5 h5d5 e2g4 h7h5 g4h5 d5h5 g1h3 h5h3 d1d3 h3g3 d3g3 f8g7 g3g7 g8h6 g7h6 h8h7 h6h7 f7f5 h7f5 b8c6 f5a5 c6a5 e1f1 a5c6 f1e1 c6b8 b1c3 e8e7";
                //string[] data = gameMoves.Split(' ');
                
                if (indexForDebug == data.Length)
                    return Console.ReadLine();
                Console.ReadLine();
                Console.WriteLine(data[indexForDebug] + " *");
                return data[indexForDebug++];
            }
            else
                return Console.ReadLine();
        }
        private string LowerToUpper(string input)
        {
            int c1 = input[0];
            int c2 = input[1];
            int c3 = input[2];
            int c4 = input[3];

            // convert small to capital letters
            if (c1 >= 'a' && c1 <= 'h')
                c1 = (char)(c1 - ('a' - 'A'));
            if (c3 >= 'a' && c3 <= 'h')
                c3 = (char)(c3 - ('a' - 'A'));
            string res = String.Format("{0}{1}{2}{3}", (char)c1, (char)c2, (char)c3, (char)c4);

            return res;
        }
        private bool IsInputFormatValid(string input)
        {
            if (input.Length != 4)
            {
                return false;
            }
            int c1 = input[0];
            int c2 = input[1];
            int c3 = input[2];
            int c4 = input[3];
            bool isChar1Legal = c1 >= 'A' && c1 <= 'H' || c1 >= 'a' && c1 <= 'h';
            bool isChar2Legal = c2 >= '1' && c2 <= '8';
            bool isChar3Legal = c3 >= 'A' && c3 <= 'H' || c3 >= 'a' && c3 <= 'h';
            bool isChar4Legal = c4 >= '1' && c4 <= '8';
            if (!(isChar1Legal && isChar2Legal && isChar3Legal && isChar4Legal))
            {
                return false;
            }
            return true;
        }
        private bool IsValidMove(string input, bool isWhite)
        {
            if (!IsInputFormatValid(input))
            {
                Console.WriteLine("Wrong input format, Please enter another move:");
                return false;
            }

            //find the cell
            Piece piece = GetPieceToMove(input);
            if (piece == null)
            {
                Console.WriteLine("Cell is empty, Please enter another move:");
                return false;
            }
            if (piece.IsWhite != isWhite)
            {
                Console.WriteLine("You are moving the wrong color, Please enter another move:");
                return false;
            }

            // check if the destination is one of the possibilites of the piece
            piece.FindAllReachablePositions();
            Position stepDestPos = CalcDestinationPosition(input);
            
            if (!piece.IsPosInPositions(stepDestPos.row, stepDestPos.col))
            {
                Console.WriteLine("Piece can't reach this position, Please enter another move:");
                return false;
            }
            // check if the step will not make the king in danger
            if (!IsStepSafeForKing(piece.cell.Pos.row, piece.cell.Pos.col, stepDestPos.row, stepDestPos.col))
            {
                Console.WriteLine("Illegal move: king will be in danger, Please enter another move:");
                return false;
            }
            return true;
        }
        protected string GetInputFromUser(bool isWhite)
        {
            bool wrongInput = true;
            string input = "";
            int stepOrigRow, stepOrigCol, stepDestRow = -1, stepDestCol = -1;
            Cell stepOrigCell;
            Piece piece = null;

            Console.WriteLine(String.Format("\n{0} turn, Please enter your move:", isWhite ? "White" : "Black"));
            while (wrongInput)
            {
                wrongInput = true;
                input = GetInput().Trim();
                input = LowerToUpper(input);
                if (IsValidMove(input, isWhite))
                    wrongInput = false;
            }
            return input;
            
        }
        private bool IsStateReapeted3Times(string state)
        {
            int counter = 0;
            foreach (string str in boardHistory)
            {
                if (str == state)
                    counter++;
            }
            return counter >= 2;
        }

        private void SaveState(string state)
        {
            string[] newBoardHistory = new string[boardHistory.Length + 1];

            for (int i = 0; i < boardHistory.Length; i++)
            {
                newBoardHistory[i] = boardHistory[i];
            }
            newBoardHistory[newBoardHistory.Length - 1] = state;
            boardHistory = newBoardHistory;
        }

        private string CreateState()
        {
            string state = "";
            foreach (Piece piece in whiteArmy.Pieces)
            {
                state += piece.CreateState();
            }
            foreach (Piece piece in blackArmy.Pieces)
            {
                state += piece.CreateState();
            }
            return state;
        }

        private bool IsDeadPosition()
        {
            int whiteBishopsCounter = 0, whiteKnightsCounter = 0;
            int blackBishopsCounter = 0, blackKnightsCounter = 0;
            int alivePiecesCounter = 0; // count the number of alive soldiers other than kings
           
            // iterate on all the pieces
            for (int i = 0; i < 16; i++)
            {
                if (i != 4)// skip the king
                {
                    if (whiteArmy.Pieces[i].IsActive)
                    {
                        if ((whiteArmy.Pieces[i] as Bishop) != null)
                            whiteBishopsCounter++;
                        else if ((whiteArmy.Pieces[i] as Knight) != null)
                            whiteKnightsCounter++;
                        else
                            alivePiecesCounter++;
                    }
                    if (blackArmy.Pieces[i].IsActive)
                    {
                        if ((blackArmy.Pieces[i] as Bishop) != null)
                            blackBishopsCounter++;
                        else if ((blackArmy.Pieces[i] as Knight) != null)
                            blackKnightsCounter++;
                        else
                            alivePiecesCounter++;
                    }
                }
            }
            // if there are pieces other than bishop and knight it is not dead position
            if (alivePiecesCounter > 0)
                return false;

            // if there are more than a single bishop or knight in any of the armies - than it is not a dead position
            if (whiteBishopsCounter > 1 || blackBishopsCounter > 1 || whiteKnightsCounter > 1 || blackKnightsCounter > 1)
                return false;

            // if there are both bishop and knight on the same army - than it is not a dead position
            if (whiteBishopsCounter != 0 && whiteKnightsCounter != 0 || blackBishopsCounter != 0 && blackKnightsCounter != 0)
                return false;

            return true;
        }
        
        internal void ResetFiftyMovesCounter()
        {
            FiftyMovesCounter = 0;
        }
    }
    class Position
    {
        public int row;
        public int col;
        public Position(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }
    class Cell
    {
        private Position pos;
        public Position Pos { get { return pos; } }
        private bool isThreatenedByWhite;
        public bool IsThreatenedByWhite { get { return isThreatenedByWhite; } set { isThreatenedByWhite = value; } }
        private bool isThreatenedByBlack;
        public bool IsThreatenedByBlack { get { return isThreatenedByBlack; } set { isThreatenedByBlack = value; } }

        public Cell(int x, int y)
        { pos = new Position(x, y); }
        private Piece _piece;
        public Piece piece { get { return _piece; } set { _piece = value; } }
        public  override string ToString()
        {
            string s = "";
            if (piece == null)
                s += "  |";
            else
            {
                string icon = piece.GetIcon();
                s += (icon + " |");
            }
            return s;
        }
        public bool IsCellEmpty()
        {
            return _piece == null;
        }
        public bool HasEnemy(bool isWhite)
        {
            if (_piece == null) return false;

            return isWhite != piece.IsWhite;
        }
        public bool IsPieceOnCurrCellThreatenedByEnemy()
        {
            if (piece == null) return false;
            return piece.IsWhite ? isThreatenedByBlack : IsThreatenedByWhite;
        }

        internal void SetCellThreat(bool isWhite)
        {
            if (isWhite)
                IsThreatenedByWhite = true;
            else
                IsThreatenedByBlack = true;
        }

        internal void ClearThreats()
        {
            IsThreatenedByBlack = false;
            IsThreatenedByWhite = false;
        }
    }
    class Board
    {
        private Cell[,] cells;
        public Cell[,] Cells { get { return cells; } }
        public Board()
        {
            cells = new Cell[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    cells[i, j] = new Cell(i, j);
                }
            }
        }

        public override string ToString()
        {
            string s = "";
            s = "\n";
            for (int i = 0; i < 8; i++)
            {
                s += (8 - i);
                for (int j = 0; j < 8; j++)
                {
                    s += cells[i, j].ToString();
                }
                s += "\n";
            }
            s += " ";
            for (int i = 0; i < 8; i++)
            {
                char c = 'A';
                c = (char)(c + i);
                s += (c + "  ");
            }
            s += "\n";
            return s;
        }

        internal void ClearAllThreatenedCells()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    cells[i, j].ClearThreats();
                }
            }
        }

        internal void CleanBoard()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    cells[i, j].piece = null;
                }
            }
        }
    }
    class Piece
    {
        protected bool isWhite;
        public bool IsWhite { get { return isWhite; } }
        protected bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
            }
        }
        protected Cell _cell;
        public Cell cell { get { return _cell; } set { _cell = value; } }
        protected Position[] reachableDestinations;
        protected int reachableDestinationsCounter;
        protected Board board;
        public Piece(bool isWhite, Board board)
        {
            this.isWhite = isWhite;
            this.board = board;
            isActive = true;
            reachableDestinations = new Position[64];
        }

        public virtual string GetIcon()
        { return ""; }
        public virtual string CreateState()
        { return ""; }

        
        public virtual Position[] FindAllReachablePositions()
        {
            throw new NotImplementedException();
        }

        protected bool IsPosInBoard(int row, int col)
        {
            if (row >= 0 && row <= 7 && col >= 0 && col <= 7)
            { return true; }
            return false;
        }

        // this method is used in evaluation mode
        internal bool IsThreateningEnemyKing()
        {
            FindAllReachablePositions();

            foreach (Position pos in reachableDestinations)
            {
                if (pos == null)
                    break;
                // check if the enemy king is in this position
                Piece pieceAtPos = board.Cells[pos.row, pos.col].piece;
                if (pieceAtPos != null && pieceAtPos is King && pieceAtPos.isWhite != isWhite)
                    return true;
            }
            return false;
        }

        protected void CleanPositionsArray()
        {
            for (int i = 0; i < reachableDestinations.Length; i++)
                reachableDestinations[i] = null;
            reachableDestinationsCounter = 0;
        }

        protected bool AddPosition(int destRow, int destCol)
        {
            Cell destCell = board.Cells[destRow, destCol];
            if (destCell.IsCellEmpty())
            {
                reachableDestinations[reachableDestinationsCounter++] = board.Cells[destRow, destCol].Pos;
                return true;
            }
            else
            {
                if (destCell.HasEnemy(isWhite))
                {
                    reachableDestinations[reachableDestinationsCounter++] = board.Cells[destRow, destCol].Pos;
                }
                return false;
            }
        }
        public bool IsPosInPositions(int row, int col)
        {
            foreach (Position pos in reachableDestinations)
            {
                if (pos == null)
                    return false;

                if (pos.row == row && pos.col == col)
                    return true;
            }
            return false;
        }

        protected void RellocatePiece(Cell destCell)
        {
            // connect the destination cell to the piece
            destCell.piece = this;

            // diconnect the curr cell from the piece
            cell.piece = null;

            // connect the piece to the destination cell
            cell = destCell;
        }

        // this is actual move that is made after all checks were finished
        internal virtual void PlayMove(int row, int col)
        {
            Cell destCell = board.Cells[row, col];
            // check if there is a piece on the cell and kill it
            if (destCell.piece != null)
                destCell.piece.IsActive = false;

            RellocatePiece(destCell);
        }

        internal void SetAllThreatenedCells()
        {
            FindAllReachablePositions();
            foreach (Position pos in reachableDestinations)
            {
                if (pos == null)
                    return;
                board.Cells[pos.row, pos.col].SetCellThreat(isWhite);
            }
        }
    }
    class Pawn : Piece
    {
        bool hasNeverMoved;
        int indexInArmy;
        public int IndexInArmy { get { return indexInArmy; } }
        bool isEnPassantAllowed;
        Position enPassantTargetPos;
        public Position EnPassantTargetPos { set { enPassantTargetPos = value; } }
        public bool IsEnPassantAllowed { set { isEnPassantAllowed = value; } }
        public Pawn(bool isWhite, Board board, int indexInArmy) : base(isWhite, board)
        {
            hasNeverMoved = true;
            this.indexInArmy = indexInArmy;
            isEnPassantAllowed = false;
        }
        public override string GetIcon()
        {
            return isWhite ? "♙" : "♟";
        }

        private void FindMovingSteps()
        {
            Cell[,] cells = board.Cells;
            int direction = isWhite ? -1 : 1;
            int row = cell.Pos.row;
            int col = cell.Pos.col;

            // check single step
            int destRow = row + direction, destCol = col;
            if (IsPosInBoard(destRow, destCol) && cells[destRow, destCol].IsCellEmpty())
                reachableDestinations[reachableDestinationsCounter++] = cells[destRow, destCol].Pos;
            
            if (hasNeverMoved)
            {
                // check two steps
                destRow = row + direction * 2;
                destCol = col;

                int middleRow = row + direction;
                int middleCol = col;
                if (IsPosInBoard(destRow, destCol) && cells[destRow, destCol].IsCellEmpty() && cells[middleRow, middleCol].IsCellEmpty())
                    reachableDestinations[reachableDestinationsCounter++] = cells[destRow, destCol].Pos;
            }
        }
        private void FindEatingSteps()
        {
            Cell[,] cells = board.Cells;
            int direction = isWhite ? -1 : 1;
            int row = cell.Pos.row;
            int col = cell.Pos.col;

            // check eating in diagonal 1
            int destRow = row + direction;
            int destCol = col + 1;
            if (IsPosInBoard(destRow, destCol) && cells[destRow, destCol].HasEnemy(IsWhite))
                reachableDestinations[reachableDestinationsCounter++] = cells[destRow, destCol].Pos;
            
            // check eating in diagonal 2
            destRow = row + direction;
            destCol = col - 1;
            if (IsPosInBoard(destRow, destCol) && cells[destRow, destCol].HasEnemy(IsWhite))
                reachableDestinations[reachableDestinationsCounter++] = cells[destRow, destCol].Pos;
        }

        // insert all possible destinations in the positions array
        public override Position[] FindAllReachablePositions()
        {
            CleanPositionsArray();

            FindMovingSteps();

            if (isEnPassantAllowed)
                reachableDestinations[reachableDestinationsCounter++] = enPassantTargetPos;
            
            FindEatingSteps();

            return reachableDestinations;
        }
        private void PlayMoveEnPassant()
        {
            Cell destCell = board.Cells[enPassantTargetPos.row, enPassantTargetPos.col];

            RellocatePiece(destCell);

            // kill the enemy pawn
            int enemyPawnRow = isWhite ? 3 : 4;
            int enemyPawnCol = enPassantTargetPos.col;
            board.Cells[enemyPawnRow, enemyPawnCol].piece.IsActive = false;
            board.Cells[enemyPawnRow, enemyPawnCol].piece = null;
        }
        internal override void PlayMove(int row, int col)
        {
            bool isPawnMovingTwoCells = Math.Abs(cell.Pos.row - row) == 2;

            // corner case: En Passant
            if (isEnPassantAllowed && row == enPassantTargetPos.row && col == enPassantTargetPos.col)
            {
                PlayMoveEnPassant();
                return;
            }

            Cell destCell = board.Cells[row, col];

            // check if there is a piece on the cell and kill it
            if (destCell.piece != null)
                destCell.piece.IsActive = false;

            RellocatePiece(destCell);

            hasNeverMoved = false;

            // handle En Passant
            if (isPawnMovingTwoCells)
            {
                HandleEnPassant(row, col);
            }
        }

        private void HandleEnPassant(int destRow, int destCol)
        {
            NotifyEnemyAboutEnPassant(destRow, destCol, destRow, destCol + 1);
            NotifyEnemyAboutEnPassant(destRow, destCol, destRow, destCol - 1);
        }
        private void NotifyEnemyAboutEnPassant(int destRow, int destCol, int enemyRow, int enemyCol)
        {
            // notify enemy pawn that it can eat me
            int enemyPawnRow = enemyRow;
            int enemyPawnCol = enemyCol;
            if (IsPosInBoard(enemyPawnRow, enemyPawnCol))
            {
                Piece enemyPiece = board.Cells[enemyPawnRow, enemyPawnCol].piece;
                if (enemyPiece != null && enemyPiece is Pawn)
                {
                    Pawn enemyPawn = enemyPiece as Pawn;
                    enemyPawn.IsEnPassantAllowed = true;

                    // calculate EnPassant destination
                    int enemyEnePassantDestRow = isWhite ? 5 : 2;
                    int enemyEnePassantDestCol = destCol;

                    enemyPawn.enPassantTargetPos = board.Cells[enemyEnePassantDestRow, enemyEnePassantDestCol].Pos;
                }
            }
        }

        public override string CreateState()
        {
            string state = String.Format("{0}{1}{2}{3}", cell.Pos.row, cell.Pos.col, hasNeverMoved ? "y" : "n", isEnPassantAllowed ? "y" : "n");
            return state;
        }
    }
    class Rook : Piece
    {
        bool hasNeverMoved;
        public bool HasNeverMoved { get { return hasNeverMoved; } set { hasNeverMoved = value; } }
        public Rook(bool isWhite, Board board) : base(isWhite, board)
        {
            hasNeverMoved = true;
        }
        public override string GetIcon()
        {
            return isWhite ? "♖" : "♜";
        }
        public override Position[] FindAllReachablePositions()
        {
            Cell[,] cells = board.Cells;
            int row = cell.Pos.row, col = cell.Pos.col, destRow, destCol;
            bool shouldContinue;
            CleanPositionsArray();

            // increase row index
            for (destRow = row + 1, destCol = col, shouldContinue = true; shouldContinue && destRow < 8; destRow++)
                shouldContinue = AddPosition(destRow, destCol);
           
            // decrease row index
            for (destRow = row - 1, destCol = col, shouldContinue = true; shouldContinue && destRow >= 0; destRow--)
                shouldContinue = AddPosition(destRow, destCol);
            
            // increase col index
            for (destRow = row, destCol = col + 1, shouldContinue = true; shouldContinue && destCol < 8; destCol++)
                shouldContinue = AddPosition(destRow, destCol);
            
            // decrease col index
            for (destRow = row, destCol = col - 1, shouldContinue = true; shouldContinue && destCol >= 0; destCol--)
                shouldContinue = AddPosition(destRow, destCol);

            return reachableDestinations;
        }
        internal override void PlayMove(int row, int col)
        {
            base.PlayMove(row, col);
            hasNeverMoved = false;
        }
        public override string CreateState()
        {
            string pieceState = String.Format("{0}{1}{2}", cell.Pos.row, cell.Pos.col, hasNeverMoved ? "y" : "n");
            return pieceState;
        }
    }
    class Knight : Piece
    {
        public Knight(bool isWhite, Board board) : base(isWhite, board)
        { }
        public override string GetIcon()
        {
            return isWhite ? "♘" : "♞";
        }
        public override Position[] FindAllReachablePositions()
        {
            CleanPositionsArray();

            Cell[,] cells = board.Cells;

            int row = cell.Pos.row;
            int col = cell.Pos.col;
            int destRow;
            int destCol;
            int[] destDiffRows = new int[8] { -2, -2, -1, -1, +1, +1, +2, +2 };
            int[] destDiffCols = new int[8] { -1, +1, -2, +2, +2, -2, +1, -1 };
            for (int i = 0; i < destDiffRows.Length; i++)
            {
                destRow = row + destDiffRows[i];
                destCol = col + destDiffCols[i];
                if (IsPosInBoard(destRow, destCol))
                {
                    Cell destCell = board.Cells[destRow, destCol];
                    if (destCell.IsCellEmpty() || destCell.HasEnemy(isWhite))
                    {
                        reachableDestinations[reachableDestinationsCounter++] = board.Cells[destRow, destCol].Pos;
                    }
                }
            }
            return reachableDestinations;
        }
        public override string CreateState()
        {
            string state = String.Format("{0}{1}", cell.Pos.row, cell.Pos.col);
            return state;
        }
    }
    class Bishop : Piece
    {
        public Bishop(bool isWhite, Board board) : base(isWhite, board)
        { }
        public override string GetIcon()
        {
            return isWhite ? "♗" : "♝";
        }
        public override Position[] FindAllReachablePositions()
        {
            CleanPositionsArray();

            Cell[,] cells = board.Cells;
            int row = cell.Pos.row;
            int col = cell.Pos.col;
            int destRow;
            int destCol;
            bool shouldContinue;
            // increase row, increase col
            for (destRow = row + 1, destCol = col + 1, shouldContinue = true; shouldContinue && destRow < 8 && destCol < 8; destRow++, destCol++)
                shouldContinue = AddPosition(destRow, destCol);
            
            // decrease row, decrease col
            for (destRow = row - 1, destCol = col - 1, shouldContinue = true; shouldContinue && destRow >= 0 && destCol >= 0; destRow--, destCol--)
                shouldContinue = AddPosition(destRow, destCol);
            
            // increase row, decrease col
            for (destRow = row + 1, destCol = col - 1, shouldContinue = true; shouldContinue && destRow < 8 && destCol >= 0; destRow++, destCol--)
                shouldContinue = AddPosition(destRow, destCol);
            
            // decrease row, increase col
            for (destRow = row - 1, destCol = col + 1, shouldContinue = true; shouldContinue && destRow >= 0 && destCol < 8; destRow--, destCol++)
                shouldContinue = AddPosition(destRow, destCol);

            return reachableDestinations;
        }
        public override string CreateState()
        {
            string state = String.Format("{0}{1}", cell.Pos.row, cell.Pos.col);
            return state;
        }
    }
    class Queen : Piece
    {
        public Queen(bool isWhite, Board board) : base(isWhite, board)
        { }
        public override string GetIcon()
        {
            return isWhite ? "♕" : "♛";
        }

        public override Position[] FindAllReachablePositions()
        {
            CleanPositionsArray();

            Cell[,] cells = board.Cells;

            int row = cell.Pos.row;
            int col = cell.Pos.col;
            int destRow;
            int destCol;
            bool shouldContinue = false;

            for(int rowDelta = -1; rowDelta < 2; rowDelta++ )
                for(int colDelta = -1; colDelta < 2; colDelta++ )
                    if(rowDelta != 0 || colDelta != 0)
                        for (destRow = row + rowDelta, destCol = col + colDelta, shouldContinue = true; shouldContinue && destRow < 8 && destRow >=0 && destCol < 8 && destCol >= 0; destRow+=rowDelta, destCol+=colDelta)
                            shouldContinue = AddPosition(destRow, destCol);

            return reachableDestinations;
        }
        public override string CreateState()
        {
            string state = String.Format("{0}{1}", cell.Pos.row, cell.Pos.col);
            return state;
        }
    }
    class King : Piece
    {
        bool hasNeverMoved;
        public bool HasNeverMoved { get { return hasNeverMoved; } set { hasNeverMoved = value; } }
        public King(bool isWhite, Board board) : base(isWhite, board)
        {
            hasNeverMoved = true;
        }
        public override string GetIcon()
        {
            return isWhite ? "♔" : "♚";
        }
        public override Position[] FindAllReachablePositions()
        {
            CleanPositionsArray();

            Cell[,] cells = board.Cells;

            int row = cell.Pos.row;
            int col = cell.Pos.col;

            // check single step
            int destRow;
            int destCol;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i != 0 || j != 0)
                    {
                        destRow = row + i;
                        destCol = col + j;
                        if (IsPosInBoard(destRow, destCol))
                        {
                            if (cells[destRow, destCol].IsCellEmpty() || cells[destRow, destCol].HasEnemy(isWhite))
                                reachableDestinations[reachableDestinationsCounter++] = cells[destRow, destCol].Pos;
                        }
                    }
                }
            }
            HandleCastling(row, col);
            
            return reachableDestinations;
        }

        private void HandleCastling(int row, int col)
        {
            if (IsShortCastlingPosible())
            {
                int destRow = row;
                int destCol = col + 2;
                reachableDestinations[reachableDestinationsCounter++] = board.Cells[destRow, destCol].Pos;
            }
            if (IsLongCastlingPosible())
            {
                int destRow = row;
                int destCol = col - 2;
                reachableDestinations[reachableDestinationsCounter++] = board.Cells[destRow, destCol].Pos;
            }
        }

        private bool IsLongCastlingPosible()
        {
            int row = cell.Pos.row, col = cell.Pos.col, rookRow = row, rookCol = 0;

            // check if king or rook never moved before
            Rook rook = board.Cells[rookRow, rookCol].piece as Rook;
            if (hasNeverMoved == false || rook == null || rook.HasNeverMoved == false)
                return false;

            // check if cells between king and rook are empty
            if (board.Cells[row, col - 1].piece != null || board.Cells[row, col - 2].piece != null || board.Cells[row, col - 3].piece != null)
                return false;

            // check for threats
            if (isWhite)
            {
                if (board.Cells[row, col].IsThreatenedByBlack || board.Cells[row, col - 1].IsThreatenedByBlack || board.Cells[row, col - 2].IsThreatenedByBlack)
                    return false;
            }
            else
            {
                if (board.Cells[row, col].IsThreatenedByWhite || board.Cells[row, col - 1].IsThreatenedByWhite || board.Cells[row, col - 2].IsThreatenedByWhite)
                    return false;
            }

            return true;
        }
        private bool IsShortCastlingPosible()
        {
            int row = cell.Pos.row, col = cell.Pos.col, rookRow = row, rookCol = 7;

            // check if king or rook never moved before
            Rook rook = board.Cells[rookRow, rookCol].piece as Rook;
            if (hasNeverMoved == false || rook == null || rook.HasNeverMoved == false)
                return false;

            // check if cells between king and rook are empty
            if (board.Cells[row, col + 1].piece != null || board.Cells[row, col + 2].piece != null)
                return false;

            // check for threats
            if (isWhite)
            {
                if (board.Cells[row, col].IsThreatenedByBlack || board.Cells[row, col + 1].IsThreatenedByBlack || board.Cells[row, col + 2].IsThreatenedByBlack)
                    return false;
            }
            else
            {
                if (board.Cells[row, col].IsThreatenedByWhite || board.Cells[row, col + 1].IsThreatenedByWhite || board.Cells[row, col + 2].IsThreatenedByWhite)
                    return false;
            }
            return true;
        }

        internal override void PlayMove(int row, int col)
        {
            int currRow = cell.Pos.row;
            int currCol = cell.Pos.col;

            // if the move is castling move the rook
            if (Math.Abs(currCol - col) >= 2)
            {
                int rookRow, rookCol, rookDestRow, rookDestCol;
                // short castling
                if (col > currCol)
                {
                    rookRow = currRow;
                    rookCol = 7;

                    rookDestRow = currRow;
                    rookDestCol = 5;
                }
                else
                {
                    rookRow = currRow;
                    rookCol = 0;

                    rookDestRow = currRow;
                    rookDestCol = 3;
                }
                Rook rook = board.Cells[rookRow, rookCol].piece as Rook;
                rook.PlayMove(rookDestRow, rookDestCol);
            }
            base.PlayMove(row, col);
            hasNeverMoved = false;
        }
        public override string CreateState()
        {
            string state = String.Format("{0}{1}{2}", cell.Pos.row, cell.Pos.col, hasNeverMoved);
            return state;
        }
    }
    class Army
    {
        private bool isWhite;
        private Piece[] pieces;
        public Piece[] Pieces { get { return pieces; } }
        private Board _board;
        public Board board { get { return _board; } }
        public Army(bool isWhite, Board board)
        {
            this.isWhite = isWhite;
            this._board = board;

            pieces = new Piece[16];
            pieces[0] = new Rook(isWhite, board);
            pieces[1] = new Knight(isWhite, board);
            pieces[2] = new Bishop(isWhite, board);
            pieces[3] = new Queen(isWhite, board);
            pieces[4] = new King(isWhite, board);
            pieces[5] = new Bishop(isWhite, board);
            pieces[6] = new Knight(isWhite, board);
            pieces[7] = new Rook(isWhite, board);
            for (int i = 8; i < 16; i++)
            {
                pieces[i] = new Pawn(isWhite, board, i);
            }
        }

        internal void Init()
        {
            int innerRow = isWhite ? 6 : 1;
            int outerRow = isWhite ? 7 : 0;
            // set the pieces on the board in the initial positions
            for (int i = 0; i < 8; i++)
            {
                // connect the piece to the cell
                pieces[i].cell = board.Cells[outerRow, i];

                // connect the cell to the piece
                board.Cells[outerRow, i].piece = pieces[i];

                // connect the piece to the cell
                pieces[i + 8].cell = board.Cells[innerRow, i];

                // connect the cell to the piece
                board.Cells[innerRow, i].piece = pieces[i + 8];
            }
        }

        public void CopyAllPositions(Army other)
        {
            Piece[] otherPieces = other.pieces;
            for (int i = 0; i < otherPieces.Length; i++)
            {
                pieces[i].IsActive = otherPieces[i].IsActive;
                if (otherPieces[i].IsActive)
                {
                    int row = otherPieces[i].cell.Pos.row;
                    int col = otherPieces[i].cell.Pos.col;
                    // connect the piece to the cell
                    pieces[i].cell = board.Cells[row, col];
                    // connet the cell to the piece
                    board.Cells[row, col].piece = pieces[i];
                }
            }
        }

        public bool IsThreateningEnemyKing()
        {
            foreach (Piece piece in pieces)
            {
                if (piece.IsActive != false)
                {
                    if (piece.IsThreateningEnemyKing())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // set all the threatened cells by this army
        public void SetAllThreatenedCells()
        {
            foreach (Piece piece in pieces)
            {
                if (piece.IsActive)
                    piece.SetAllThreatenedCells();
            }
        }

        internal void ClearEnPassantPriviliges()
        {
            for (int i = 8; i < 16; i++)
            {
                if (pieces[i] != null)
                {
                    Pawn pawn = pieces[i] as Pawn;
                    if (pawn != null)
                        pawn.IsEnPassantAllowed = false;
                }

            }
        }
    }
}