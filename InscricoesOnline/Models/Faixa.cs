using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InscricoesOnline.Models
{
    [Table("TBL_FAIXAS")]
    public class Faixa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        [MaxLength(80)]
        public string Descricao { get; set; }

        [Required]
        [Display(Name = "Ordem")]
        public int Ordem { get; set; }

        public DateTime DataRegistro { get; set; }

        public long EventoId { get; set; }
        public virtual Evento Evento { get; set; }
    }
}