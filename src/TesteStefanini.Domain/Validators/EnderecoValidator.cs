using FluentValidation;
using TesteStefanini.Domain.Entities;

namespace TesteStefanini.Domain.Validators
{
    public class EnderecoValidator : AbstractValidator<Endereco>
    {
        public EnderecoValidator()
        {
            // Validação do Logradouro (obrigatório)
            RuleFor(e => e.Logradouro)
                .NotEmpty().WithMessage("O logradouro é obrigatório.")
                .MaximumLength(100).WithMessage("O logradouro não pode ter mais de 100 caracteres.");

            // Validação do Número (obrigatório)
            RuleFor(e => e.Numero)
                .NotEmpty().WithMessage("O número é obrigatório.")
                .MaximumLength(10).WithMessage("O número não pode ter mais de 10 caracteres.");

            // Validação do Complemento (opcional)
            RuleFor(e => e.Complemento)
                .MaximumLength(100).WithMessage("O complemento não pode ter mais de 100 caracteres.");

            // Validação do Bairro (obrigatório)
            RuleFor(e => e.Bairro)
                .NotEmpty().WithMessage("O bairro é obrigatório.")
                .MaximumLength(50).WithMessage("O bairro não pode ter mais de 50 caracteres.");

            // Validação da Cidade (obrigatório)
            RuleFor(e => e.Cidade)
                .NotEmpty().WithMessage("A cidade é obrigatória.")
                .MaximumLength(50).WithMessage("A cidade não pode ter mais de 50 caracteres.");

            // Validação do Estado (obrigatório)
            RuleFor(e => e.Estado)
                .NotEmpty().WithMessage("O estado é obrigatório.")
                .MaximumLength(2).WithMessage("Use a sigla do estado com 2 caracteres.");

            // Validação do CEP (obrigatório e formato válido)
            RuleFor(e => e.CEP)
                .NotEmpty().WithMessage("O CEP é obrigatório.")
                .Matches(@"^\d{5}-?\d{3}$").WithMessage("O CEP deve estar no formato 00000-000 ou 00000000.");

            // Validação do ID da Pessoa (obrigatório)
            RuleFor(e => e.PessoaId)
                .NotEmpty().WithMessage("O ID da pessoa é obrigatório.");
        }
    }
}
