namespace TestProject1
//create steps with:
{//pgn-extract.exe "larsen,_bent_vs_fischer,_robert_james.pgn" -Wuci > myout.txt
    public class Tests
    {
        TextWriter buOut;
        StringWriter sw;
        [SetUp]
        public void Setup()
        {
            TalpiotChessGame.ChessGameLauncher.DEBUG_MODE = false;
            buOut = Console.Out;
            sw = TalpiotChessGame.TestUtils.sw = new StringWriter();
        }
        [TearDown]
        public void Teardown()
        {
            Console.SetOut(buOut);
        }
        [Test]
        public void TestFoolsMate()
        {
            var data = String.Join(Environment.NewLine, new[]
            {
                "F2F3",
                "E7E5",
                "G2G4",
                "D8H4",
            });
            string Expected = "Winner is: Black";
            using (var sr = new StringReader(data))
            {
                Console.SetIn(sr);
                TalpiotChessGame.ChessGameLauncher.Main(["test"]);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }

        [Test]
        public void TestShortCastlingWhite()
        {
            var data = String.Join(Environment.NewLine, new[]
            {
                "G2G4",
                "A7A6",
                "F1H3",
                "B7B6",
                "G1F3",
                "C7C6",
                "E1G1",
                "E7E6",
                "G1H1",
                "D8H4",
                "B2B4",
                "A6A5",
                "B4A5",
                "A8A5",
                "G4G5",
                "A5G5",
                "D1E1",
                "H4G4",
                "B1C3",
                "G4G2",
                "H3G2",
                "H7H5",
                "F3D4",
                "H5H4",
                "C3B1",
                "H4H3",
                "G2H3",
                "H8H3",
                "D4C6",
                "H3H2",
                "H1H2",
                "F8D6",
                "H2H3",
                "D6G3",
                "F2F3",
                "G8F6",
                "A2A3",
                "F6D5",
                "A3A4",
                "D5F4",
            });
            string Expected = "Winner is: Black";
            using (var sr = new StringReader(data))
            {
                Console.SetIn(sr);
                TalpiotChessGame.ChessGameLauncher.Main(["test"]);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
        [Test]
        public void TestLongCastlingWhite()
        {
            var data = String.Join(Environment.NewLine, new[]
            {
                "B1A3",
                "A7A6",
                "D2D4",
                "B7B6",
                "C1E3",
                "C7C6",
                "D1D2",
                "D7D6",
                "E1C1",
                "B6B5",
                "A3C4",
                "D8A5",
                "H2H3",
                "A5A2",
                "H1H2",
                "A6A5",
                "C4A5",
                "A8A5",
                "H2H1",
                "A2A1",
            });
            string Expected = "Winner is: Black";
            using (var sr = new StringReader(data))
            {
                Console.SetIn(sr);
                TalpiotChessGame.ChessGameLauncher.Main(["test"]);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
        [Test]
        public void TestThreeFoldRepetition1()
        {
            var data = String.Join(Environment.NewLine, new[]
            {
                "G1H3",
                "G8H6",
                "H3G1",
                "H6G8",
                "G1H3",
                "G8H6",
                "H3G1",
                "H6G8",
                "G1H3",
                "G8H6",
                "H3G1",
                "H6G8",
                "G1H3",
                "G8H6",
                "H3G1",
                "H6G8",
                "G1H3",
                "G8H6",
                "H3G1",
                "H6G8",
                "G1H3",
                "G8H6",
                "H3G1",
                "H6G8",
                "G1H3",
                "G8H6",
                "H3G1",
                "H6G8",
                "G1H3",
                "G8H6",
                "H3G1",
                "H6G8",
                "G1H3",
                "G8H6",
                "H3G1",
                "H6G8",
                "G1H3",
                "G8H6",
                "H3G1",
                "H6G8",
                "G1H3",
                "G8H6",
                "H3G1",
                "H6G8",
                "G1H3",
                "G8H6",
                "H3G1",
                "H6G8",
                "G1H3",
                "G8H6",
                "H3G1",
                "H6G8",
                "G1H3",
                "G8H6",
                "H3G1",
                "H6G8",
                "G1H3",
                "G8H6",
                "H3G1",
                "H6G8",
                "G1H3",
                "G8H6",
                "H3G1",
                "H6G8",
            });
            string Expected = "Three fold repetition - Game finished with a draw";
            using (var sr = new StringReader(data))
            {
                Console.SetIn(sr);
                TalpiotChessGame.ChessGameLauncher.Main(["test"]);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
        [Test]
        public void TestThreeFoldRepetition2()
        {
            string gameMoves = "b1c3 b8c6 a1b1 a8b8 b1a1 b8a8 a1b1 a8b8 b1a1 b8a8 a1b1 a8b8";
            string[] arr = gameMoves.Split(' ');
            var data = String.Join(Environment.NewLine, arr);

            string Expected = "Three fold repetition - Game finished with a draw";
            using (var sr = new StringReader(data))
            {
                Console.SetIn(sr);
                TalpiotChessGame.ChessGameLauncher.Main(["test"]);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
        [Test]
        public void TestFiftyMovesRule()
        {
            var data = String.Join(Environment.NewLine, new[]
            {
                "A2A4",
                "H7H5",
                "A1A3",
                "H8H6",
                "A3H3",
                "H6A6",
                "H3H5",
                "A6A4",
                "H5H6",
                "A4A3",
                "H6G6",
                "A3B3",
                "G6F6",
                "B3D3",
                "F6E6",
                "D3E3",
                "E6D6",
                "E3F3",
                "D6C6",
                "F3G3",
                "C6B6",
                "G3H3",
                "B6A6",
                "H3H4",
                "A6A5",
                "H4H5",
                "A5A4",
                "H5H6",
                "A4A3",
                "H6G6",
                "A3B3",
                "G6F6",
                "B3D3",
                "F6E6",
                "D3E3",
                "E6D6",
                "E3F3",
                "D6C6",
                "F3G3",
                "C6B6",
                "G3H3",
                "B6A6",
                "H3H4",
                "A6A5",
                "H4H5",
                "A5A4",
                "B1C3",//horse
                "G8F6",//horse
                "C3E4",//horse
                "F6D5",//horse  
                "H5H6",
                "A4A3",
                "H6G6",
                "A3B3",
                "G6F6",
                "B3D3",
                "F6E6",
                "D3E3",
                "E6D6",
                "E3F3",
                "D6C6",
                "F3G3",
                "C6B6",
                "G3H3",
                "B6A6",
                "H3H4",
                "A6A5",
                "H4H5",
                "A5A4",
                "H5H6",
                "A4A3",
                "H6G6",
                "A3B3",
                "G6F6",
                "B3D3",
                "F6E6",
                "D3E3",
                "E6D6",
                "E3F3",
                "D6C6",
                "F3G3",
                "C6B6",
                "G3H3",
                "B6A6",
                "H3H4",
                "A6A5",// last good!
                "H4H5",
                "A5A4",
                "H5H6",
                "A4A3",
                "H6G6",
                "A3B3",
                "G6F6",
                "B3D3",
                "F6E6",
                "D3E3",
                "E6D6",
                "E3F3",
                "D6C6",
                "F3G3",
                "C6B6",
                "G3H3",
                "B6A6",
                "H3H4",
                "A6A5",
                "E4C5",
                "D5F4",
                "H4H5",
                "A5A4",
                "H5H6",
                "A4A3",
                "H6G6",
                "A3B3",
                "G6F6",
                "B3D3",
                "F6E6",
                "D3E3",
                "E6D6",
                "E3F3",
                "D6C6",
                "F3G3",
                "C6B6",
                "G3H3",
                "B6A6",
                "H3H4",
                "A6A5",

            });
            string Expected = "Game is Stagnant for 50 turns - It's a Draw!";
            using (var sr = new StringReader(data))
            {
                Console.SetIn(sr);
                TalpiotChessGame.ChessGameLauncher.Main(["test"]);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }

        [Test]
        public void TestBobbyFisher()
        {
            string gameMoves = "e2e4 e7e5 g1f3 b8c6 f1b5 a7a6 b5a4 g8f6 e1g1 f8e7 f1e1 b7b5 a4b3 d7d6 c2c3 e8g8 h2h3 c8b7 d2d4 c6a5 b3c2 a5c4 b2b3 c4b6 b1d2 b6d7 b3b4 e5d4 c3d4 a6a5 b4a5 c7c5 e4e5 d6e5 d4e5 f6d5 d2e4 d5b4 c2b1 a8a5 d1e2 d7b6 f3g5 b7e4 e2e4 g7g6 e4h4 h7h5 h4g3 b6c4 g5f3 g8g7 g3f4 f8h8 e5e6 f7f5 b1f5 d8f8 f5e4 f8f4 c1f4 h8e8 a1d1 a5a6 d1d7 a6e6 f3g5 e6f6 e4f3 f6f4 g5e6 g7f6 e6f4 c4e5 d7b7 e7d6 g1f1 b4c2 e1e4 c2d4 b7b6 e8d8 f4d5 f6f5 d5e3 f5e6 f3e2 e6d7 e2b5 d4b5 b6b5 d7c6 a2a4 d6c7 f1e2 g6g5 g2g3 d8a8 b5b2 a8f8 f2f4 g5f4 g3f4 e5f7 e4e6 f7d6 f4f5 f8a8 b2d2 a8a4 f5f6 c7b6 h3h4 a4a2 d2a2 b6a5 a2a5 c5c4 f6f7 c6c7 e3d5 c7b8 a5c5 d6b5 e2e3 b5a7 e3e2 b8a8 c5b5 c4c3 e6e8 a7c8 e8e7 c3c2 e2e1 c8d6 e7d7 d6f5 d5e7 f5e3 e7c6 e3d1 d7d8";
            string[] arr = gameMoves.Split(' ');

            var data = String.Join(Environment.NewLine, arr);
            string Expected = "Winner is: White";
            using (var sr = new StringReader(data))
            {
                Console.SetIn(sr);
                TalpiotChessGame.ChessGameLauncher.Main(["test"]);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }

        [Test]
        public void TestPromotion()
        {
            string gameMoves = "a2a4 b7b5 a4b5 b8c6 b5b6 c8a6 b6b7 c6e5 b7b8 1 a6d3 b8a8 d8c8 a8c8";
            string[] arr = gameMoves.Split(' ');
            var data = String.Join(Environment.NewLine, arr);

            string Expected = "Winner is: White";
            using (var sr = new StringReader(data))
            {
                Console.SetIn(sr);
                TalpiotChessGame.ChessGameLauncher.Main(["test"]);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
        [Test]
        public void TestEnPassant()
        {
            string gameMoves = "b2b4 g7g6 b4b5 c7c5 b5c6 f7f5 b1a3 f5f4 e2e4 f4e3 d2e3 h7h6 e3e4 h8h7 h2h4 g6g5 h4g5 h6g5 h1h7 b8a6 d1h5";
            string[] arr = gameMoves.Split(' ');
            var data = String.Join(Environment.NewLine, arr);

            string Expected = "Winner is: White";
            using (var sr = new StringReader(data))
            {
                Console.SetIn(sr);
                TalpiotChessGame.ChessGameLauncher.Main(["test"]);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
        [Test]
        public void TestDeadPositionKingVsKing()
        {
            string gameMoves = "a2a4 b7b5 a4b5 a7a5 b5a6 a8a6 a1a5 a6b6 a5a7 b6b2 a7c7 b2b1 c7c8 b1c1 c8b8 c1c2 b8b7 c2d2 b7d7 d2d3 d7d6 d3f3 d6f6 f3f2 f6f7 f2g2 f7g7 g2g1 g7g8 g1h1 g8h8 h1h2 h8h7 h2e2 e1e2 f8h6 h7h6 d8d1 e2f2 d1f1 f2f1 e7e6 h6e6 e8d8 e6e7 d8e7";
            string[] arr = gameMoves.Split(' ');
            var data = String.Join(Environment.NewLine, arr);

            string Expected = "Dead position - Game finished with a draw";
            using (var sr = new StringReader(data))
            {
                Console.SetIn(sr);
                TalpiotChessGame.ChessGameLauncher.Main(["test"]);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
        [Test]
        public void TestDeadPositionKingsAndBishops()
        {
            string gameMoves = "a2a4 a7a5 a1a3 a8a6 a3h3 a6h6 h3g3 h6h2 g3g7 h2h1 g7g8 h1g1 g8h8 g1g2 h8h7 g2f2 h7f7 f2f3 f7f6 f3d3 f6d6 d3d2 d6d7 d2c2 d7c7 c2c1 c7c8 c1b1 c8b8 b1b2 b8b7 b2a2 b7a7 a2a4 a7a5 e7e5 e2e4 a4e4 d1e2 d8e7 a5e5 e4e2 f1e2 e7e6 e5e6 e8f7 e6d6 f8d6";

            string[] arr = gameMoves.Split(' ');
            var data = String.Join(Environment.NewLine, arr);

            string Expected = "Dead position - Game finished with a draw";
            using (var sr = new StringReader(data))
            {
                Console.SetIn(sr);
                TalpiotChessGame.ChessGameLauncher.Main(["test"]);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
        [Test]
        public void TestDeadPositionKingVsKingAndBishop()
        {
            string gameMoves = "a2a4 b7b5 a1a3 c7c5 a4b5 a7a6 b5a6 a8a6 a3h3 a6a2 h3h7 a2b2 h7h8 b2b1 h8g8 b1c1 d1c1 g7g5 g8h8 c8a6 h8h7 d8a5 h7f7 a5a3 f7e7 e8d8 g2g3 a3g3 g1h3 g3h3 c2c3 h3h2 d2d4 h2h1 e7f7 h1g2 f2f3 g2f3 f7f6 f3c3 e1f2 c3b3 c1c5 a6b5 c5b5 b8a6 b5a6 f8a3 a6a3 d8e8 a3a5 d7d5 a5d5 b3b4 e2e4 b4d4 f2e1 d4f6 d5g5 f6e5 f1e2 e5e4 g5e5 e4e5 e1f1 e5b5 e2b5";

            string[] arr = gameMoves.Split(' ');
            var data = String.Join(Environment.NewLine, arr);

            string Expected = "Dead position - Game finished with a draw";
            using (var sr = new StringReader(data))
            {
                Console.SetIn(sr);
                TalpiotChessGame.ChessGameLauncher.Main(["test"]);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
        [Test]
        public void TestDeadPositionKingVsKingAndKnight()
        {
            string gameMoves = "g2g3 g8h6 f1h3 e7e6 g1f3 f8a3 e1g1 e8g8 g1h1 g8h8 f3g1 h6g8 f2f4 h7h5 h3f5 h5h4 g3h4 e6f5 e2e4 f5e4 f1f3 e4f3 g1f3 g7g5 f3g5 f7f5 g5f3 d8h4 f3g1 d7d5 c2c4 d5c4 b2b4 c4b3 a2b3 f8f6 h2h3 a3c1 b1a3 c1a3 b3b4 a3b4 a1a7 h4f4 a7b7 f6g6 b7b8 g6g4 h3g4 f5g4 b8c8 a8a1 d1a1 b4c3 d2c3 f4c4 c8b8 c7c6 b8c8 c6c5 c8c5 g4g3 a1e1 c4b4 e1g3 b4c5 g3f3 c5c3 f3e3 c3e3 g1h3 g8h6 h3g1 e3g3 g1f3 g3g4 f3g1 g4g1 h1g1";

            string[] arr = gameMoves.Split(' ');
            var data = String.Join(Environment.NewLine, arr);

            string Expected = "Dead position - Game finished with a draw";
            using (var sr = new StringReader(data))
            {
                Console.SetIn(sr);
                TalpiotChessGame.ChessGameLauncher.Main(["test"]);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
        [Test]
        public void TestDeadPositionKingAndKnightVsKingAndKnight()
        {
            string gameMoves = "f2f4 e7e5 f4e5 a7a5 e5e6 a5a4 e6e7 a4a3 b2a3 a8a3 c1a3 b7b5 c2c4 b5c4 a3c5 c8a6 a2a4 a6b5 c5d4 b5a4 a1a4 c7c5 d4c5 d7d6 c5d6 d8a5 a4c4 a5a6 c4d4 a6d6 d4d3 d6d3 h2h4 g7g5 e2e4 d3d6 f1e2 d6e5 g2g4 e5e4 h1h3 e4g4 h3f3 g4f3 h4h5 f3h5 d2d4 g5g4 d4d5 h5d5 e2g4 h7h5 g4h5 d5h5 g1h3 h5h3 d1d3 h3g3 d3g3 f8g7 g3g7 g8h6 g7h6 h8h7 h6h7 f7f5 h7f5 b8c6 f5a5 c6a5 e1f1 a5c6 f1e1 c6b8 b1c3 e8e7";

            string[] arr = gameMoves.Split(' ');
            var data = String.Join(Environment.NewLine, arr);

            string Expected = "Dead position - Game finished with a draw";
            using (var sr = new StringReader(data))
            {
                Console.SetIn(sr);
                TalpiotChessGame.ChessGameLauncher.Main(["test"]);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
        [Test]
        public void TestStalemate()
        {
            string gameMoves = "a2a4 h7h5 h2h4 h8h6 g1f3 g8f6 f3e5 e7e6 e2e3 f8a3 b2a3 e8f8 h1h3 f6e4 f1a6 b8a6 e1f1 f8g8 f1g1 g8h8 g1h1 d8g8 d1g1 g7g5 g2g4 h5g4 h4g5 a6b4 a3b4 b7b5 a4b5 f7f5 f2f4 e4g5 h3h4 h6h7 h4h2 g5h3 h2e2 h7e7 c2c4 d7d5 c4d5 e6d5 e3e4 d5e4 d2d3 e4d3 e5d3 h3f4 e2e7 c8a6 b5a6 a8b8 a1a5 b8b4 a5f5 b4a4 f5f4 g4g3 d3b4 a4b4 f4f2 g3g2 f2g2 b4a4 e7c7 a4a6 c7a7 a6a1 g2b2 a1b1 b2c2 b1c1 c2c7 c1c7 a7a8 c7c8 a8a7 c8a8 a7a2 g8a2 g1b6 a8a6 b6a6 a2a5 a6a5 h8g8 h1h2 g8h8 h2h3 h8g8 h3h4 g8h8 h4h5 h8g8 a5e1 g8h8 e1f1 h8g8 h5h6 g8h8 f1f7";
            string[] arr = gameMoves.Split(' ');
            var data = String.Join(Environment.NewLine, arr);

            string Expected = "It's a Pat - Game finished with a draw";
            using (var sr = new StringReader(data))
            {
                Console.SetIn(sr);
                TalpiotChessGame.ChessGameLauncher.Main(["test"]);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
        [Test]
        public void TestPat2()
        {
            string gameMoves = "a2a4 a7a5 a1a3 a8a6 a3b3 a6a8 b3b7 a8a7 b7b8 a7a8 b8c8 a8a7 c8c7 a7a8 c7d7 a8a7 d7d6 a7a8 d6f6 a8a7 f6f7 a7a8 f7g7 a8a7 g7g8 a7a8 g8h8 a8a7 h8h7 a7a8 h7h6 a8a7 h6g6 f8h6 g6h6 e7e6 h6e6 d8e7 e6e7 e8f8 e7e5 a7a6 e5a5 a6a5 b2b4 a5a4 d2d4 a4b4 e2e4 b4b5 f1c4 b5b1 c4f1 b1b3 c1b2 b3b2 h2h4 b2c2 h1h3 c2c3 h3g3 c3g3 h4h5 g3g2 f2f4 g2g4 g1e2 g4f4 d1c1 f4e4 c1d1 e4d4 h5h6 d4h4 h6h7 h4h7 f1h3 h7g7 h3d7 g7g8 e2f4 g8g7 d1d5 g7g8 f4h5 g8g7 e1d1 g7g8 d5e6 g8h8 e6e3 h8g8 e3d3 g8g3 d3g3 f8f7 g3e3 f7f8 e3e6";

            string[] arr = gameMoves.Split(' ');
            var data = String.Join(Environment.NewLine, arr);

            string Expected = "It's a Pat - Game finished with a draw";
            using (var sr = new StringReader(data))
            {
                Console.SetIn(sr);
                TalpiotChessGame.ChessGameLauncher.Main(["test"]);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
        [Test]
        public void TestPat3()
        {
            string gameMoves = "a2a4 a7a5 a1a3 a8a6 a3b3 a6a8 b3b7 a8a7 b7b8 a7a8 b8c8 a8a7 c8c7 a7a8 c7d7 a8a7 d7d6 a7a8 d6f6 a8a7 f6f7 a7a8 f7g7 a8a7 g7g8 a7a8 g8h8 a8a7 h8h7 a7a8 h7h6 a8a7 h6g6 f8h6 g6h6 e7e6 h6e6 d8e7 e6e7 e8f8 e7e5 a7a6 e5a5 a6a5 b2b4 a5a4 d2d4 a4b4 e2e4 b4b5 f1c4 b5b1 c4f1 b1b3 c1b2 b3b2 h2h4 b2c2 h1h3 c2c3 h3g3 c3g3 h4h5 g3g2 f2f4 g2g4 g1e2 g4f4 d1c1 f4e4 c1d1 e4d4 h5h6 d4h4 h6h7 h4h3 f1h3 f8e8 h3f1 e8e7 e2d4 e7d8 f1d3 d8c8 e1d2 c8b8 d2c3 b8a8 c3b4 a8b8 b4b5 b8a8 d4c6 a8b7 d1e1 b7a8 b5b6";

            string[] arr = gameMoves.Split(' ');
            var data = String.Join(Environment.NewLine, arr);

            string Expected = "It's a Pat - Game finished with a draw";
            using (var sr = new StringReader(data))
            {
                Console.SetIn(sr);
                TalpiotChessGame.ChessGameLauncher.Main(["test"]);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
        [Test]
        public void TestBobbyFisher2()
        {
            string gameMoves = "d2d4 g8f6 g1f3 g7g6 g2g3 f8g7 f1g2 e8g8 e1g1 d7d6 b1c3 b8d7 b2b3 e7e5 d4e5 d6e5 e2e4 f8e8 c1a3 c7c6 a3d6 d8a5 d1d3 e8e6 b3b4 a5a3 d6c7 a3b4 a1b1 b4e7 f1d1 f6e8 c7a5 e6d6 d3e2 d6d1 e2d1 g7f8 f3d2 e7a3 d2c4 a3c5 g2f1 b7b5 c4d2 c5a3 d2b3 d7c5 f1b5 c6b5 c3b5 a3a4 b3c5 a4a5 d1d5 a8b8 a2a4 c8h3 d5e5 b8c8 c5d3 a5a4 d3e1 a7a6 e5c3 a4e4 e1f3 e4f3 b1b4 f3g2";
            string[] arr = gameMoves.Split(' ');
            var data = String.Join(Environment.NewLine, arr);

            string Expected = "Winner is: Black";
            using (var sr = new StringReader(data))
            {
                Console.SetIn(sr);
                TalpiotChessGame.ChessGameLauncher.Main(["test"]);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
        [Test]
        public void TestDangerousEnPassant()
        {
            string gameMoves = "e2e3 e7e5 e1e2 e5e4 h2h4 g7g5 h1h3 g5h4 h3h4 a7a6 h4h7 f7f5 h7h3 f5f4 a2a3 f4e3 h3e3 d2d4 a6a5 d2d4 e4d3 d8h4 g2g3 f8b4 a3b4 g8h6 g3h4 c7c5 c2c3 c5b4 c3c4 d1b3 b4b3 d1b3 a5a4 b3b4 a4a3 e3g3 a3a2 g3g7 b4e7 e4e3 b4e7";
            string[] arr = gameMoves.Split(' ');
            var data = String.Join(Environment.NewLine, arr);

            string Expected = "Winner is: White";
            using (var sr = new StringReader(data))
            {
                Console.SetIn(sr);
                TalpiotChessGame.ChessGameLauncher.Main(["test"]);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
        [Test]
        public void TestLongCastlingWhenMostLeftIsThreatened()
        {
            string gameMoves = "a2a4 a7a5 c2c4 c7c5 b2b4 c5b4 b1c3 b4b3 c1a3 b3b2 d2d4 b2b1 1 e2e4 b1c2 d1d3 b8a6 g1h3 c2a2 e1c1 b7b6 a3c5 b6c5 d3f3 a8b8 g2g4 a2b2";
            string[] arr = gameMoves.Split(' ');
            var data = String.Join(Environment.NewLine, arr);

            string Expected = "Winner is: Black";
            using (var sr = new StringReader(data))
            {
                Console.SetIn(sr);
                TalpiotChessGame.ChessGameLauncher.Main(["test"]);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
        [Test]
        public void TestStalemate2()
        {
            string[] arr = {
"c2c4", "h7h5", "h2h4", "a7a5", "d1a4",
"a8a6", "a4a5", "a6h6", "a5c7", "f7f6",
"c7d7", "e8f7", "d7b7", "d8d3", "b7b8",
"d3h7", "b8c8", "f7g6", "c8e6" };
            var data = String.Join(Environment.NewLine, arr);

            string Expected = "It's a Pat - Game finished with a draw";
            using (var sr = new StringReader(data))
            {
                Console.SetIn(sr);
                TalpiotChessGame.ChessGameLauncher.Main(["test"]);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
        [Test]
        public void Test50MovesRuleNoEatsButPawnsMove()// the pawns DID move but wasn't recognized by the counter
        {
            string input = "a2a3 a7a6 b2b3 b7b6 c2c3 c7c6 d2d3 d7d6 e2e3 e7e6 f2f3 f7f6 g2g3 g7g6 h2h3 h7h6 a3a4 a6a5 b3b4 b6b5 c3c4 c6c5 d3d4 d6d5 e3e4 e6e5 f3f4 f6f5 g3g4 g6g5 h3h4 h6h5 g1f3 b8c6 f3d2 c6e7 d2b3 e7g6 b3d2 g6e7 d2f3 e7c6 f3h2 c6a7 f1h3 c8a6 h2f1 a7c8 f1d2 c8e7 d2b3 e7g6 b3d2 g6e7 d2f3 e7c6 f3e5 c6d4 e5f3 d4c6 f3g5 c6b4 d1d4 b4a2 d4f6 a6b7 f6f7";
            string[] arr = input.Split(' ');
            var data = String.Join(Environment.NewLine, arr);

            string Expected = "Winner is: White";
            using (var sr = new StringReader(data))
            {
                Console.SetIn(sr);
                TalpiotChessGame.ChessGameLauncher.Main(["test"]);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
    }
}
