using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeaponStats.Models;
using WeaponStats.Models.Census;

namespace WeaponStats.Services.Environment {

    /// <summary>
    /// Background queue to manage updating characters. When a character ID is placed into the queue it will be
    ///     updated eventually, but no guarantee about when is made. <see cref="Queue(CharacterUpdateParameters)"/>
    ///     and <see cref="Dequeue(CancellationToken)"/>are not meant to be ran on the same thread,
    ///     as Dequeueing will block until something has been queued
    /// </summary>
    public interface IBackgroundCharacterQueue {

        /// <summary>
        ///     Queue a new character ID to be updated
        /// </summary>
        /// <param name="param">Parameters used to determine when to update the character</param>
        void Queue(CharacterUpdateParameters param);

        /// <summary>
        ///     Are there queued entries
        /// </summary>
        bool IsEmpty();

        /// <summary>
        ///     Get how many character IDs are in the queue
        /// </summary>
        int Length();

        /// <summary>
        ///     Remove the next character ID that's been queued. If the queue is empty, it will block
        ///     until a new value has been queued. 
        /// </summary>
        /// <param name="cancel">Cancellaction token</param>
        /// <returns>The next character ID to be processed</returns>
        Task<CharacterUpdateParameters> Dequeue(CancellationToken cancel);

    }
}
