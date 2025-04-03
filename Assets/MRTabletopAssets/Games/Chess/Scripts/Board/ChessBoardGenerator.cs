using System.Collections.Generic;
using UnityEngine;

namespace UnityLabs.Slices.Games.Chess
{
    public class ChessBoardGenerator : MonoBehaviour
    {
        [HideInInspector]
        public ChessBoardTile[] AllSquaresGO = new ChessBoardTile[64];

        [SerializeField]
        ChessBoardTile m_TilePrefab = null;

        [HideInInspector]
        [SerializeField]
        float m_BoardWidth;

        [HideInInspector]
        [SerializeField]
        float m_HalfBoardWidth;

        [SerializeField]
        Transform m_BlackGraveyardRoot = null;

        public Transform blackGraveyardRoot => m_BlackGraveyardRoot;

        [SerializeField]
        Transform m_WhiteGraveyardRoot = null;

        public Transform whiteGraveyardRoot => m_WhiteGraveyardRoot;

        public float tileWidth { get; private set; }

        readonly Dictionary<ChessSquare, ChessBoardTile> m_PositionMap = new Dictionary<ChessSquare, ChessBoardTile>(64);
        readonly List<ChessBoardTile> m_PositionList = new List<ChessBoardTile>(64);
        bool m_Initialized = false;

        public Dictionary<ChessSquare, ChessBoardTile> positionMap
        {
            get
            {
                if (!m_Initialized) GenerateBoard();
                return m_PositionMap;
            }
        }

        public List<ChessBoardTile> positionList
        {
            get
            {
                if (!m_Initialized) GenerateBoard();
                return m_PositionList;
            }
        }

        public ChessSquare GetSquareClosestToPos(Vector3 pos)
        {
            ChessSquare closestSquare = default;
            float distance = float.MaxValue;

            foreach (var tile in positionMap)
            {
                var tileDist = Vector3.SqrMagnitude(tile.Value.initialLocalPosition - pos);
                if (tileDist < distance)
                {
                    distance = tileDist;
                    closestSquare = tile.Key;
                }
            }

            var distFromBlackGraveyard = Vector3.SqrMagnitude(m_BlackGraveyardRoot.localPosition - pos);
            if (distFromBlackGraveyard < distance)
            {
                distance = distFromBlackGraveyard;
                closestSquare = ChessSquare.BlackGraveyard;
            }

            var distFromWhiteGraveyard = Vector3.SqrMagnitude(m_WhiteGraveyardRoot.localPosition - pos);
            if (distFromWhiteGraveyard < distance)
            {
                distance = distFromWhiteGraveyard;
                closestSquare = ChessSquare.WhiteGraveyard;
            }

            return closestSquare;
        }

        void OnEnable()
        {
            if (m_PositionMap == null)
            {
                GenerateBoard();
            }
        }

        void GenerateBoard()
        {
            m_Initialized = true;

            var tileInstance = Instantiate(m_TilePrefab);
            tileWidth = tileInstance.tileWidth;
            m_BoardWidth = tileWidth * 7f;
            m_HalfBoardWidth = m_BoardWidth / 2f;

            // Clear template instance
            Destroy(tileInstance.gameObject);

            m_PositionMap.Clear();
            m_PositionList.Clear();
            Transform boardTransform = transform;

            for (int file = 1; file <= 8; file++)
            {
                for (int rank = 1; rank <= 8; rank++)
                {
                    var newSquareTileInstance = Instantiate(m_TilePrefab);
                    newSquareTileInstance.gameObject.name = ChessSquareUtil.FileRankToSquareString(file, rank);
                    // newSquareTileInstance.tag = "Square";

                    var locakPosition = new Vector3(FileOrRankToSidePosition(file), 0f, FileOrRankToSidePosition(rank));
                    var chessSquare = new ChessSquare(file, rank);
                    var chessColor = (file + rank) % 2 == 0 ? ChessColor.Black : ChessColor.White;

                    newSquareTileInstance.Initialize(chessSquare, chessColor, locakPosition, boardTransform);

                    m_PositionMap.Add(chessSquare, newSquareTileInstance);
                    m_PositionList.Add(newSquareTileInstance);
                    AllSquaresGO[(file - 1) * 8 + (rank - 1)] = newSquareTileInstance;
                }
            }
        }

        private float FileOrRankToSidePosition(int index)
        {
            float t = (index - 1) / 7f;
            return Mathf.Lerp(-m_HalfBoardWidth, m_HalfBoardWidth, t);
        }
    }
}
