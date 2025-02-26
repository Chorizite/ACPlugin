﻿using Chorizite.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC.API.WorldObjects {
    public class Equippable : Item {
        /// <summary>
        /// The wielder of this equipment, if any
        /// </summary>
        public Creature? Wielder => ACPlugin.Instance.Game.World.Get((uint)Value(PropertyInstanceId.Wielder)) as Creature;

        public Equippable() {
        }
    }
}
