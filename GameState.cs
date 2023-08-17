namespace Tetris
{
    public class GameState
    {
        private Block currentBlock;

        public Block CurrentBlock
        {
            get => currentBlock;
            private set
            {
                currentBlock = value;
                currentBlock.Reset();

                for (int i = 0; i < 2; i++)
                {
                    currentBlock.Move(1, 0);

                    if (!BlockFits()) CurrentBlock.Move(-1, 0);
                }
            }
        }

        public GameGrid GameGrid { get; }
        public BlockQueue blockQueue { get; }
        public bool GameOver { get; private set; }

        public int Score { get; private set; }

        public double Speed { get; set; }
        public int Freeze { get; set; }

        public Block HeldBlock { get; private set; }

        public bool CanHold { get; private set; }

        public GameState()
        {
            GameGrid = new GameGrid(22, 10);
            blockQueue = new BlockQueue();
            CurrentBlock = blockQueue.GetAndUpdate();
            CanHold = true;
        }

        private bool BlockFits()
        {
            foreach (Position p in CurrentBlock.TilePositions())
            {
                if (!GameGrid.IsEmpty(p.Row, p.Column)) return false;
            }
            return true;
        }

        public void HoldBlock()
        {
            if (!CanHold)
            {
                Block tmp = CurrentBlock;
                CurrentBlock = HeldBlock;
                HeldBlock = tmp;
                return;
            }

            if (HeldBlock == null)
            {
                HeldBlock = CurrentBlock;
                CurrentBlock = blockQueue.GetAndUpdate();
            }
            else
            {
                Block tmp = CurrentBlock;
                CurrentBlock = HeldBlock;
                HeldBlock = tmp;
            }

            CanHold = false;
        }

        public void RotateBlockCW()
        {
            CurrentBlock.RotateCW();
            if (!BlockFits()) CurrentBlock.RotateCCW();
        }

        public void RotateBlockCCW()
        {
            CurrentBlock.RotateCCW();
            if (!BlockFits()) CurrentBlock.RotateCW();
        }

        public void MoveBlockLeft() 
        {
            CurrentBlock.Move(0, -1);
            if (!BlockFits()) CurrentBlock.Move(0, 1);
        }

        public void MoveBlockRight()
        {
            CurrentBlock.Move(0, 1);
            if (!BlockFits()) CurrentBlock.Move(0, -1);
        }

        private bool IsGameOver() 
        {
            return !(GameGrid.IsRowEmpty(0) && GameGrid.IsRowEmpty(1));
        }

        private void PlaceBock() 
        {
            foreach (Position p in CurrentBlock.TilePositions())
            {
                GameGrid[p.Row, p.Column] = CurrentBlock.Id;
            }

            int tmpScore = GameGrid.ClearFullRows();
            
            if (tmpScore >= 2) 
            {
                Freeze += tmpScore;
                
            }

            Score += tmpScore;

            if (IsGameOver())
            {
                GameOver = true;
            }
            else
            {
                CurrentBlock = blockQueue.GetAndUpdate();
                CanHold = true;
            }
        }

        public void MoveBlockDown()
        {
            CurrentBlock.Move(1, 0);

            if (!BlockFits())
            {
                CurrentBlock.Move(-1, 0);
                PlaceBock();
            }
        }

        private int TileDropDistance(Position p)
        {
            int drop = 0;

            while (GameGrid.IsEmpty(p.Row + drop + 1, p.Column))
            {
                drop++;
            }

            return drop;
        }

        public int BlockDropDistance()
        {
            int drop = GameGrid.Rows;

            foreach (Position p in CurrentBlock.TilePositions())
            {
                drop = System.Math.Min(drop, TileDropDistance(p));
            }

            return drop;
        }

        public void DropBlock()
        {
            CurrentBlock.Move(BlockDropDistance(), 0);
            PlaceBock();
        }
    }
}
