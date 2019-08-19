using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InscricoesOnline.Models
{
    [Table("TBL_ATLETAS")]
    public class Atleta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nome { get; set; }

        [MaxLength(20)]
        public String CPF { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataNascimento { get; set; }

        [Required]
        [MaxLength(20)]
        public String Sexo { get; set; }

        [Required]
        [Display(Name = "Ativo")]
        public bool Ativo { get; set; }

        public DateTime DataRegistro { get; set; }

        [Required]
        public bool Parataekwondo { get; set; }

        public long FaixaId { get; set; }
        public virtual Faixa Faixa { get; set; }

        public long EquipeId { get; set; }
        public virtual Equipe Equipe { get; set; }
    }
}