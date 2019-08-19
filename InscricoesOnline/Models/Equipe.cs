using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InscricoesOnline.Models
{
    [Table("TBL_EQUIPES")]
    public class Equipe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [Display(Name = "Nome")]
        [MaxLength(100)]
        public String Nome { get; set; }

        [Display(Name = "CNPJ")]
        [MaxLength(20)]
        public String CNPJ { get; set; }

        [MaxLength(50)]
        public string Login { get; set; }

        [MaxLength(30)]
        public string Senha { get; set; }

        public DateTime DataRegistro { get; set; }

        public long EventoId { get; set; }
        public virtual Evento Evento { get; set; }

        public virtual List<Atleta> Atletas { get; set; }
    }
}