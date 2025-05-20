# API TesteStefanini

## Modificações Implementadas

### Integração Pessoa e Endereço
- Modificado o modelo de dados para que uma pessoa tenha uma lista de endereços
- Simplificada a estrutura da API, removendo serviços e controladores redundantes
- Implementada a validação de endereços

### Versões da API
- **Versão 1 (v1)**: Mantém todos os endpoints (GET, POST, PUT, DELETE) e o endereço é opcional
- **Versão 2 (v2)**: Mantém apenas os endpoints POST e PUT, e o endereço é obrigatório

### Endpoints Disponíveis

#### Versão 1 (v1)
- `GET /api/v1/pessoas` - Listar todas as pessoas com seus endereços
- `GET /api/v1/pessoas/{id}` - Obter pessoa por ID com seus endereços
- `POST /api/v1/pessoas` - Criar nova pessoa (endereços opcionais)
- `PUT /api/v1/pessoas/{id}` - Atualizar pessoa (endereços opcionais)
- `DELETE /api/v1/pessoas/{id}` - Excluir pessoa

#### Versão 2 (v2)
- `POST /api/v2/pessoas` - Criar nova pessoa (pelo menos um endereço obrigatório)
- `PUT /api/v2/pessoas/{id}` - Atualizar pessoa (pelo menos um endereço obrigatório)

## Autenticação
A API utiliza autenticação JWT. Para obter um token, faça uma requisição POST para `/api/auth/login` com as credenciais de acesso.
