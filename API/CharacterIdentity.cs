﻿using System;

namespace AC.API {
    /// <summary>
    /// Represents a character identity.
    /// </summary>
    public class CharacterIdentity {
        /// <summary>
        /// The character id
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        /// The character name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The DateTime when the character will be deleted. This is only valid if
        /// <see cref="IsBeingDeleted"/> is true
        /// </summary>
        public DateTime DeletionTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Whether the character is being deleted
        /// </summary>
        public bool IsBeingDeleted { get; set; }

        public CharacterIdentity() { }

        internal CharacterIdentity(Chorizite.ACProtocol.Types.CharacterIdentity c) {
            Id = c.CharacterId;
            Name = c.Name;
            IsBeingDeleted = c.SecondsGreyedOut > 0;
            
            if (IsBeingDeleted) {
                DeletionTime = DateTime.Now + TimeSpan.FromSeconds(c.SecondsGreyedOut);
            }
        }

    }
}