using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InscricoesOnline.Models
{
    [Table("TBL_MODALIDADES")]
    public class Modalidade
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MaxLength(50)]
        public String Titulo { get; set; }

        [Required]
        public bool CategoriaPeso { get; set; }

        [Required]
        public bool CategoriaFaixa { get; set; }

        [Required]
        public bool CategoriaIdade { get; set; }

        [Required]
        public bool Ranking { get; set; }

        [Required]
        public bool Parataekwondo { get; set; }

        public long EventoId { get; set; }
        public virtual Evento Evento { get; set; }
    }
}