﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Pokemon_Forum_API.Entities
{
    public class Forums
    {
        #region Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int forum_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }

        public List<SubForums> subforums { get; set; } = new List<SubForums>();

        #endregion

        #region Constructor

        public Forums() { }

        public Forums(string name, string description)
        {
            this.name = name;
            this.description = description;
        }

        public Forums(int forum_id, string name, string description)
        {
            this.forum_id = forum_id;
            this.name = name;
            this.description = description;
        }

        #endregion
    }
}
