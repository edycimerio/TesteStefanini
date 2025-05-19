using System;
using System.Text.RegularExpressions;
using FluentValidation;
using TesteStefanini.Domain.Entities;
using TesteStefanini.Domain.Interfaces;

namespace TesteStefanini.Domain.Validators
{
    public class PessoaValidator : AbstractValidator<Pessoa>
    {
        private readonly IPessoaRepository _pessoaRepository;

        public PessoaValidator(IPessoaRepository pessoaRepository = null)
        {
            _pessoaRepository = pessoaRepository;

            // Validação do Nome (obrigatório)
            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MaximumLength(100).WithMessage("O nome não pode ter mais de 100 caracteres.");

            // Validação do Email (opcional, mas deve ser válido se preenchido)
            RuleFor(p => p.Email)
                .EmailAddress().WithMessage("O e-mail informado não é válido.")
                .When(p => !string.IsNullOrEmpty(p.Email));

            // Validação da Data de Nascimento (obrigatória e não pode ser futura)
            RuleFor(p => p.DataNascimento)
                .NotEmpty().WithMessage("A data de nascimento é obrigatória.")
                .LessThanOrEqualTo(DateTime.Today).WithMessage("A data de nascimento não pode ser no futuro.");

            // Validação do CPF (obrigatório e deve estar em formato válido)
            RuleFor(p => p.CPF)
                .NotEmpty().WithMessage("O CPF é obrigatório.")
                .Must(BeAValidCPF).WithMessage("O CPF informado não é válido.");

            // Validação de unicidade do CPF (assíncrona)
            RuleFor(p => p.CPF)
                .MustAsync(async (pessoa, cpf, cancellation) => 
                {
                    if (_pessoaRepository == null) return true;
                    return !await _pessoaRepository.CpfJaExisteAsync(cpf, pessoa.Id);
                }).WithMessage("Este CPF já está cadastrado para outra pessoa.")
                .When(p => !string.IsNullOrEmpty(p.CPF) && BeAValidCPF(p.CPF));
        }

        // Método para validar o formato do CPF
        private bool BeAValidCPF(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
                return false;

            // Remove caracteres não numéricos
            cpf = Regex.Replace(cpf, "[^0-9]", "");

            // CPF deve ter 11 dígitos
            if (cpf.Length != 11)
                return false;

            // Verifica se todos os dígitos são iguais (CPF inválido)
            bool allDigitsEqual = true;
            for (int i = 1; i < cpf.Length; i++)
            {
                if (cpf[i] != cpf[0])
                {
                    allDigitsEqual = false;
                    break;
                }
            }
            if (allDigitsEqual)
                return false;

            // Cálculo do primeiro dígito verificador
            int sum = 0;
            for (int i = 0; i < 9; i++)
                sum += int.Parse(cpf[i].ToString()) * (10 - i);

            int remainder = sum % 11;
            int digit1 = remainder < 2 ? 0 : 11 - remainder;

            // Verifica o primeiro dígito verificador
            if (int.Parse(cpf[9].ToString()) != digit1)
                return false;

            // Cálculo do segundo dígito verificador
            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += int.Parse(cpf[i].ToString()) * (11 - i);

            remainder = sum % 11;
            int digit2 = remainder < 2 ? 0 : 11 - remainder;

            // Verifica o segundo dígito verificador
            return int.Parse(cpf[10].ToString()) == digit2;
        }
    }
}
