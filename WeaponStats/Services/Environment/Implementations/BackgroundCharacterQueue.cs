using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeaponStats.Models;

namespace WeaponStats.Services.Environment.Implementations {

    public class BackgroundCharacterQueue : IBackgroundCharacterQueue {

        private ConcurrentQueue<CharacterUpdateParameters> _Items = new ConcurrentQueue<CharacterUpdateParameters>();

        private SemaphoreSlim _Signal = new SemaphoreSlim(0);

        public void Queue(CharacterUpdateParameters param) {
            if (param == null) {
                throw new ArgumentNullException(nameof(param));
            }

            _Items.Enqueue(param);
            _Signal.Release();
        }

        public bool IsEmpty() {
            return _Items.IsEmpty;
        }

        public int Length() {
            return _Items.Count;
        }

        public async Task<CharacterUpdateParameters> Dequeue(CancellationToken cancel) {
            await _Signal.WaitAsync(cancel);
            _Items.TryDequeue(out CharacterUpdateParameters? param);

            return param!;
        }

    }
}
