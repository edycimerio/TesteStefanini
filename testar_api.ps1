# Script para testar a API TesteStefanini
$baseUrl = "http://localhost:5212"

Write-Host "Testando a API TesteStefanini..." -ForegroundColor Cyan

# 1. Login para obter o token
Write-Host "`n1. Testando login..." -ForegroundColor Yellow
$loginData = @{
    email = "admin@teste.com"
    senha = "Admin@123"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method Post -ContentType "application/json" -Body $loginData
    Write-Host "Login realizado com sucesso!" -ForegroundColor Green
    Write-Host "Token: $($loginResponse.Token)" -ForegroundColor Gray
    
    $token = $loginResponse.Token
    $headers = @{
        Authorization = "Bearer $token"
    }
} catch {
    Write-Host "Erro ao fazer login: $_" -ForegroundColor Red
    exit
}

# 2. Criar uma pessoa
Write-Host "`n2. Testando criação de pessoa..." -ForegroundColor Yellow
$pessoaData = @{
    nome = "João da Silva"
    sexo = "M"
    email = "joao@teste.com"
    dataNascimento = "1990-01-01"
    naturalidade = "São Paulo"
    nacionalidade = "Brasileira"
    cpf = "52998224725"
} | ConvertTo-Json

try {
    $pessoaResponse = Invoke-RestMethod -Uri "$baseUrl/api/v1/pessoas" -Method Post -Headers $headers -ContentType "application/json" -Body $pessoaData
    Write-Host "Pessoa criada com sucesso!" -ForegroundColor Green
    Write-Host "ID da pessoa: $($pessoaResponse.Id)" -ForegroundColor Gray
    
    $pessoaId = $pessoaResponse.Id
} catch {
    Write-Host "Erro ao criar pessoa: $_" -ForegroundColor Red
    $errorResponse = $_.Exception.Response
    $errorResponseStream = $errorResponse.GetResponseStream()
    $reader = New-Object System.IO.StreamReader($errorResponseStream)
    $errorResponseBody = $reader.ReadToEnd()
    Write-Host "Detalhes do erro: $errorResponseBody" -ForegroundColor Red
}

# 3. Listar todas as pessoas
Write-Host "`n3. Testando listagem de pessoas..." -ForegroundColor Yellow
try {
    $pessoas = Invoke-RestMethod -Uri "$baseUrl/api/v1/pessoas" -Method Get -Headers $headers
    Write-Host "Pessoas listadas com sucesso!" -ForegroundColor Green
    Write-Host "Total de pessoas: $($pessoas.Count)" -ForegroundColor Gray
    
    foreach ($pessoa in $pessoas) {
        Write-Host "  - $($pessoa.Nome) (ID: $($pessoa.Id))" -ForegroundColor Gray
    }
} catch {
    Write-Host "Erro ao listar pessoas: $_" -ForegroundColor Red
}

# 4. Obter uma pessoa específica
if ($pessoaId) {
    Write-Host "`n4. Testando obtenção de pessoa específica..." -ForegroundColor Yellow
    try {
        $pessoa = Invoke-RestMethod -Uri "$baseUrl/api/v1/pessoas/$pessoaId" -Method Get -Headers $headers
        Write-Host "Pessoa obtida com sucesso!" -ForegroundColor Green
        Write-Host "Nome: $($pessoa.Nome)" -ForegroundColor Gray
        Write-Host "E-mail: $($pessoa.Email)" -ForegroundColor Gray
        Write-Host "CPF: $($pessoa.CPF)" -ForegroundColor Gray
    } catch {
        Write-Host "Erro ao obter pessoa: $_" -ForegroundColor Red
    }

    # 5. Atualizar uma pessoa
    Write-Host "`n5. Testando atualização de pessoa..." -ForegroundColor Yellow
    $pessoaUpdateData = @{
        nome = "João da Silva Atualizado"
        sexo = "M"
        email = "joao.atualizado@teste.com"
        dataNascimento = "1990-01-01"
        naturalidade = "Rio de Janeiro"
        nacionalidade = "Brasileira"
    } | ConvertTo-Json

    try {
        $pessoaUpdated = Invoke-RestMethod -Uri "$baseUrl/api/v1/pessoas/$pessoaId" -Method Put -Headers $headers -ContentType "application/json" -Body $pessoaUpdateData
        Write-Host "Pessoa atualizada com sucesso!" -ForegroundColor Green
        Write-Host "Nome atualizado: $($pessoaUpdated.Nome)" -ForegroundColor Gray
        Write-Host "E-mail atualizado: $($pessoaUpdated.Email)" -ForegroundColor Gray
    } catch {
        Write-Host "Erro ao atualizar pessoa: $_" -ForegroundColor Red
    }

    # 6. Criar um endereço para a pessoa (versão 2 da API)
    Write-Host "`n6. Testando criação de pessoa com endereço (v2)..." -ForegroundColor Yellow
    $pessoaEnderecoData = @{
        nome = "Maria da Silva"
        sexo = "F"
        email = "maria@teste.com"
        dataNascimento = "1992-05-15"
        naturalidade = "São Paulo"
        nacionalidade = "Brasileira"
        cpf = "529.982.247-25"
        logradouro = "Rua das Flores"
        numero = "123"
        complemento = "Apto 101"
        bairro = "Centro"
        cidade = "São Paulo"
        estado = "SP"
        cep = "01234-567"
    } | ConvertTo-Json

    try {
        $pessoaEnderecoResponse = Invoke-RestMethod -Uri "$baseUrl/api/v2/pessoas/completo" -Method Post -Headers $headers -ContentType "application/json" -Body $pessoaEnderecoData
        Write-Host "Pessoa com endereço criada com sucesso!" -ForegroundColor Green
        Write-Host "ID da pessoa: $($pessoaEnderecoResponse.Id)" -ForegroundColor Gray
        
        $pessoaEnderecoId = $pessoaEnderecoResponse.PessoaId
    } catch {
        Write-Host "Erro ao criar pessoa com endereço: $_" -ForegroundColor Red
        $errorResponse = $_.Exception.Response
        $errorResponseStream = $errorResponse.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($errorResponseStream)
        $errorResponseBody = $reader.ReadToEnd()
        Write-Host "Detalhes do erro: $errorResponseBody" -ForegroundColor Red
        
        # Vamos imprimir o JSON que estamos enviando para depurar
        Write-Host "JSON enviado:" -ForegroundColor Yellow
        Write-Host $pessoaEnderecoData -ForegroundColor Yellow
    }

    # 7. Obter pessoa com endereço (versão 2 da API)
    if ($pessoaEnderecoId) {
        Write-Host "`n7. Testando obtenção de pessoa com endereço (v2)..." -ForegroundColor Yellow
        try {
            $pessoaEndereco = Invoke-RestMethod -Uri "$baseUrl/api/v2/pessoas/$pessoaEnderecoId/completo" -Method Get -Headers $headers
            Write-Host "Pessoa com endereço obtida com sucesso!" -ForegroundColor Green
            Write-Host "Nome: $($pessoaEndereco[0].Nome)" -ForegroundColor Gray
            Write-Host "Endereço: $($pessoaEndereco[0].Endereco.Logradouro), $($pessoaEndereco[0].Endereco.Numero)" -ForegroundColor Gray
        } catch {
            Write-Host "Erro ao obter pessoa com endereço: $_" -ForegroundColor Red
        }
    }

    # 8. Excluir uma pessoa
    Write-Host "`n8. Testando exclusão de pessoa..." -ForegroundColor Yellow
    try {
        Invoke-RestMethod -Uri "$baseUrl/api/v1/pessoas/$pessoaId" -Method Delete -Headers $headers
        Write-Host "Pessoa excluída com sucesso!" -ForegroundColor Green
    } catch {
        Write-Host "Erro ao excluir pessoa: $_" -ForegroundColor Red
    }
}

Write-Host "`nTestes concluídos!" -ForegroundColor Cyan
