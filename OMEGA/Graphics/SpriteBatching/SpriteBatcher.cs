using System;

namespace OMEGA
{
    internal class SpriteBatcher
    {
        private const int InitialBatchSize = 256;

        private const int MaxBatchSize = short.MaxValue / 6;

        private SpriteBatchItem[] _batchItemList;

        private int _batchItemCount;

        private readonly QuadVertexStream _vertexStream;

        private ShaderProgram _shader;


        public SpriteBatcher(ShaderProgram shader, int capacity = 0)
        {
            if (capacity == 0)
            {
                capacity = InitialBatchSize;
            }
            else
            {
                // Ensure chunks of 64
                capacity = (capacity + 63) & (~63);
            }

            _batchItemList = new SpriteBatchItem[capacity];

            _batchItemCount = 0;

            for (int i = 0; i < capacity; ++i)
            {
                _batchItemList[i] = new SpriteBatchItem();
            }

            _vertexStream = new QuadVertexStream(capacity);

            EnsureCapacity(capacity);

            _shader = shader;
        }

        public void SetShader(ShaderProgram shader)
        {
            _shader = shader;
        }

        public SpriteBatchItem CreateBatchItem()
        {
            if (_batchItemCount >= _batchItemList.Length)
            {
                var oldSize = _batchItemList.Length;
                var newSize = oldSize + oldSize / 2;
                newSize = (newSize + 63) & (~63);
                Array.Resize(ref _batchItemList, newSize);
                for (int i = oldSize; i < newSize; ++i)
                {
                    _batchItemList[i] = new SpriteBatchItem();
                }

                EnsureCapacity(Calc.Min(newSize, MaxBatchSize));
            }

            var item = _batchItemList[_batchItemCount++];
            return item;
        }

        private void EnsureCapacity(int numBatchItems)
        {
            _vertexStream.UpdateBuffers(numBatchItems);
        }

        public void DrawBatch(SpriteSortMode sortMode)
        {
            if (_batchItemCount == 0)
            {
                return;
            }

            switch (sortMode)
            {
                case SpriteSortMode.Texture:
                case SpriteSortMode.FrontToBack:
                case SpriteSortMode.BackToFront:

                    Array.Sort(_batchItemList, 0, _batchItemCount);

                    break;
            }

            int batchIndex = 0;
            int batchCount = _batchItemCount;

            while (batchCount > 0)
            {
                Texture2D tex = null;

                int numBatchesToProcess = batchCount;
                if (numBatchesToProcess > MaxBatchSize)
                {
                    numBatchesToProcess = MaxBatchSize;
                }

                for (int i = 0; i < numBatchesToProcess; ++i, ++batchIndex)
                {
                    SpriteBatchItem item = _batchItemList[batchIndex];

                    var shouldFlush = !ReferenceEquals(item.Texture, tex);

                    if (shouldFlush)
                    {
                        FlushVertexStream();

                        tex = item.Texture;
                        _shader.SetTexture(0, tex);
                    }

                    _vertexStream.Push(in item.Quad);

                    item.Texture = null;
                }

                FlushVertexStream();

                batchCount -= numBatchesToProcess;
            }

            _batchItemCount = 0;
        }

        private void FlushVertexStream()
        {
            if (_vertexStream.VertexCount == 0)
            {
                return;
            }

            _vertexStream.Submit();
        }
    }
}