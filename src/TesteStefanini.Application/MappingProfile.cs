using AutoMapper;
using TesteStefanini.Application.DTOs;
using TesteStefanini.Domain.Entities;

namespace TesteStefanini.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeamentos de Pessoa
            CreateMap<Pessoa, PessoaDto>();
            CreateMap<CreatePessoaDto, Pessoa>();
            CreateMap<UpdatePessoaDto, Pessoa>();
            
            // Mapeamentos de Endereço
            CreateMap<Endereco, EnderecoDto>();
            CreateMap<CreateEnderecoDto, Endereco>();
            CreateMap<UpdateEnderecoDto, Endereco>();
            
            // Mapeamentos de Usuário
            CreateMap<Usuario, UsuarioDto>();
            
            // Mapeamentos para a versão 2 da API
            CreateMap<Pessoa, PessoaEnderecoDto>();
        }
    }
}
