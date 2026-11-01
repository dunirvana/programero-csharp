# programero-csharp

Projetos de exemplo em C#/.NET dos posts do blog [PROGRAMERO](https://programero.blogspot.com).

Cada pasta é um projeto independente, ligado a um post. Para rodar qualquer um deles basta ter o Docker instalado:

```bash
cd <pasta-do-projeto>
docker compose up --build
```

E para rodar os testes:

```bash
docker compose run --rm tests
```

## Projetos

| Projeto | Post |
|---|---|
| [2026-11-rate-limiting-minimal-apis](2026-11-rate-limiting-minimal-apis/) | [Rate limiting com Minimal APIs](https://programero.blogspot.com/2026/11/rate-limiting-minimal-apis.html) |
