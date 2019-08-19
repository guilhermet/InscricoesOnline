using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InscricoesOnline.Models
{
    [Table("TBL_CATEGORIAS_PESO")]
    public class CategoriaLutaPeso
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MaxLength(50)]
        public String Titulo { get; set; }

        [Required]
        public int PesoInicial { get; set; }

        [Required]
        public int PesoFinal { get; set; }

        [Required]
        [MaxLength(10)]
        public String Sexo { get; set; }

        public long CategoriaIdadeId { get; set; }
        public virtual CategoriaIdade CategoriaIdade { get; set; }

        public bool Ativo { get; set; }

        public long EventoId { get; set; }
        public virtual Evento Evento { get; set; }
    }
}