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


            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MaximumLength(100).WithMessage("O nome não pode ter mais de 100 caracteres.");


            RuleFor(p => p.Email)
                .EmailAddress().WithMessage("O e-mail informado não é válido.")
                .When(p => !string.IsNullOrEmpty(p.Email));


            RuleFor(p => p.DataNascimento)
                .NotEmpty().WithMessage("A data de nascimento é obrigatória.")
                .LessThanOrEqualTo(DateTime.Today).WithMessage("A data de nascimento não pode ser no futuro.");


            RuleFor(p => p.CPF)
                .NotEmpty().WithMessage("O CPF é obrigatório.")
                .Must(BeAValidCPF).WithMessage("O CPF informado não é válido.");


            RuleFor(p => p.CPF)
                .MustAsync(async (pessoa, cpf, cancellation) => 
                {
                    if (_pessoaRepository == null) return true;
                    return !await _pessoaRepository.CpfJaExisteAsync(cpf, pessoa.Id);
                }).WithMessage("Este CPF já está cadastrado para outra pessoa.")
                .When(p => !string.IsNullOrEmpty(p.CPF) && BeAValidCPF(p.CPF));
        }


        private bool BeAValidCPF(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
                return false;


            cpf = Regex.Replace(cpf, "[^0-9]", "");


            if (cpf.Length != 11)
                return false;


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


            int sum = 0;
            for (int i = 0; i < 9; i++)
                sum += int.Parse(cpf[i].ToString()) * (10 - i);

            int remainder = sum % 11;
            int digit1 = remainder < 2 ? 0 : 11 - remainder;


            if (int.Parse(cpf[9].ToString()) != digit1)
                return false;


            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += int.Parse(cpf[i].ToString()) * (11 - i);

            remainder = sum % 11;
            int digit2 = remainder < 2 ? 0 : 11 - remainder;


            return int.Parse(cpf[10].ToString()) == digit2;
        }
    }
}
