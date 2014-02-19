/*
    by FabianoNalin.net.br

    :(
    XGH na veiaaa...
        -- Se contratado prometo refatorar o código antes de disponibilizá-lo aos clientes! (:
*/

//Tenta carregar  o ajax se reconhecer o hash da página
$().ready(function () {
    /*
        :(
        Frescura... só para usar o # na URL
        Se for contratado prometo q estudo mais SPA e uso os templates do VS (:
    */
    $("li#cadastro").addClass("active");

    var update = Boolean($("#update").val());
    if (update) {
        window.location.hash = "Empresa";
    } else {
        var hash = window.location.hash.replace('#', '');
        var bloco = 1;
        var lista = $("ul").find("[data-tipo='" + hash + "']");
        if (hash.length > 0 && lista.length > 0) {
            lista.addClass("active");
            CarregaDados(hash, lista.data('titulo'), bloco);
            CarregaPaginacao(bloco);
        }
    }
});

//detecta qdo há a mudança de hash na url
$(window).bind('hashchange', function (event) {
    var hash = location.hash.replace('#', '');
    var lista = $("ul").find("[data-tipo='" + hash + "']");
    if (hash.length > 0) {
        RemoverActives();
        if (lista.length > 0) {
            var update = Boolean($("#update").val());
            var bloco = 1;
            var active = 1;
            if (update) {
                var itemBloco = $("#bloco").val();
                if (itemBloco != "") {
                    //em caso de atualização
                    bloco = active = Number(itemBloco);
                } else {
                    //em caso de insert
                    bloco = 0;
                    active = "last";
                }
            }
            lista.addClass("active");
            CarregaDados(hash, lista.data('titulo'), bloco);
            CarregaPaginacao(active);
        } else {
            //se não achar conteúdo relacionado com a hash:
            $("#partial").html("<div id='erro-mensagem'><p style='color:gray;'><small><em><strong>Atenção.<br />Conteúdo não localizado...</strong></em></small></p></div>");
        }
    }
});

//retira a class active de todos
var RemoverActives = function () {
    $('.mnu-lateral').each(function () {
        $(this).find('li').each(function () {
            $(this).removeClass("active");
        });
    });
};

var CarregaDados = function (tipo, titulo, bloco) {
    if (tipo != undefined) {
        //var bloco = 1;
        var strUrl = "/Cadastro/Get" + tipo + "/" + "?bloco=" + bloco;
        $.ajax(
        {
            type: 'GET',
            url: strUrl,
            dataType: 'html',
            cache: false,
            async: true,
            beforeSend: function () {
                //$("#updateButton span").text(" Atualizando");
                $("#cabecalho-dados h2").text(titulo);
                $("#cabecalho-dados").removeClass("hide");
                $("#update i").addClass("icon-refresh-animate");
                $("#partial").html("<div id='erro-mensagem'><p style='color:gray;'><small><em><strong>Acessando Base de dados.<br />Aguarde, estou indo até o servidor... <i class='glyphicon glyphicon-refresh icon-refresh-animate'></i></strong></em></small></p></div>");
            },
            success: function (data) {
                $('#partial').hide().html(data).fadeIn("slow");
                $(".delete-btn").click(function (e) {
                    e.preventDefault();
                    var itemID = $(this).data('id');
                    var nome = $(this).data("nome");
                    $('#deleteModal').modal('show');
                    $("#deleteModal .modal-body input[type=hidden]").val(itemID);
                    $("#deleteModal .modal-body span").text(nome);
                });
                $(".contatos-btn").click(function (e) {
                    e.preventDefault();
                    var itemID = $(this).data('id');
                    var nome = $(this).data("nome");
                    $("#contatosModal .modal-body input[type=hidden]").val(itemID).data("nome", nome);
                    $('#contatosModal').modal('show');
                });
                $("#update i").removeClass("icon-refresh-animate");
            },
            error: function () {
                $("#partial").html("<div id='erro-mensagem'><p style='color:gray;'><small><em><strong>Erro.<br />Não foi possível recuperar os dados do servidor!</strong></em></small></p></div>");
                $("#update i").removeClass("icon-refresh-animate");
                $("#cabecalho-dados").addClass("hide");
            },
        });
    }
};

var CarregaPaginacao = function (active) {
    var strUrl = "/Cadastro/GetPaginacao/";
    $.ajax({
        type: 'GET',
        url: strUrl,
        dataType: 'html',
        cache: false,
        async: true,
        success: function (data) {
            $("#menuPaginacao").html(data);
            $("#menuPaginacao ul li").click(function (e) {
                e.preventDefault();
                var itemClick = $(this);
                var hash = window.location.hash.replace('#', '');
                var lista = $("ul").find("[data-tipo='" + hash + "']");
                CarregaDados(hash, lista.data("titulo"), Number(itemClick.text()));
                PagClick(itemClick);
            });
            switch (active) {
                case "last":
                    $("#menuPaginacao ul li:last-child").addClass("active");
                    break;
                case undefined:
                    $("#menuPaginacao ul li:first-child").addClass("active");
                    break;
                default:
                    $("#menuPaginacao ul li:nth-child(" + active + ")").addClass("active");
                    break;
            }
        },
        error: function () {
            $("#menuPaginacao").html("<div id='erro-mensagem'><p style='color:gray;'><small><em><strong>Erro.<br />Não foi possível obter a paginação de forma correta do servidor</strong></em></small></p></div>");
        },
    });
};

var PagClick = function (e) {
    $("#menuPaginacao ul li.active").removeClass("active");
    e.addClass("active");
};

$("a#update").click(function (e) {
    e.preventDefault();
    var atual = $("#menuPaginacao ul li.active");
    var hash = window.location.hash.replace('#', '');
    var lista = $("ul").find("[data-tipo='" + hash + "']");
    CarregaDados(hash, lista.data("tipo"), Number(atual.text()));
    PagClick(atual);
});

$("#deleteModal .modal-footer button").click(function (e) {
    e.preventDefault();
    var strUrl = "/Cadastro/DeleteEmpresa/";
    var itemID = $(".modal-body input[type=hidden]").val();
    pag = $("#menuPaginacao ul li.active").text();
    $.ajax({
        url: strUrl,
        type: 'post',
        dataType: 'json',
        data: { id: itemID },
        beforeSend: function () {
            var loading = "<span><em>Excluindo</em>&nbsp;<i class='glyphicon glyphicon-refresh icon-refresh-animate'></i></span>";
            $('#deleteModal .modal-header h4').after(loading);
        },
        success: function (data) {
            if (data.length == 0) {
                $("#msgAlerta span").text('Item excluído!');
                $("#msgAlerta").hide().removeClass('hide').fadeIn(500);
                $('#deleteModal').modal('hide');
                var linhas = $("#tabelaDados tr").length;
                if (linhas == 2) {
                    pag = Number(pag - 1);
                }
                var hash = window.location.hash.replace('#', '');
                var lista = $("ul").find("[data-tipo='" + hash + "']");
                CarregaDados(hash, lista.data("tipo"), Number(pag));
                CarregaPaginacao(pag);
            } else {
                $("#msgErroDelete span").text(data);
                $("#msgErroDelete").hide().removeClass('hide').fadeIn(500);
            }
        },
        complete: function () {
            setTimeout(function () { $("#msgAlerta").fadeOut(500); }, 3000);
            setTimeout(function () { $("#msgErroDelete").fadeOut(500); }, 3000);
            $("div.modal-header span").remove();
        },
    });
});

//executado qdo modal acionado e antes de exibí-lo
$('#contatosModal').on('show.bs.modal', function (e) {
    var loading = "<span><em>Buscando dados no servidor... </em>&nbsp;<i class='glyphicon glyphicon-refresh icon-refresh-animate'></i></span>";
    $('#contatosModal .modal-header h4').after(loading);
    var nome = $("#contatosModal .modal-body input[type=hidden]").data("nome");
    $(".nome-empresa span").text(nome);
    $(".del-contato").addClass("hidden");
    $(".mnu-btn button i").removeClass("glyphicon-remove").addClass("glyphicon-plus");
    $(".mnu-btn button").data("acao", "add");
    $(".mnu-btn ,.dados-contato").addClass("hidden");
    $('.dados-contato input').val('');
    $("#nomeContato").parent().removeClass("has-error");
    $("#contatosModal .modal-body .relacao").hide();
    $(".del-contato p .btn-danger span").text("Sim");
});

//executado qdo modal acionado e após de exibí-lo
$('#contatosModal').on('shown.bs.modal', function (e) {
    CarregaContatos();
});

var CarregaContatos = function () {
    var strUrl = "/Cadastro/GetContatos/" + $("#contatosModal .modal-body input[type=hidden]").val();
    $.ajax(
    {
        type: 'GET',
        url: strUrl,
        dataType: 'html',
        cache: false,
        async: true,
        success: function (data) {
            $("#contatosModal .modal-body .relacao").html(data).fadeIn("slow");
            $("div.modal-header span").remove();
            $(".mnu-btn").removeClass("hidden");
            $(".del-contato-btn").on("click", function (e) {
                var nome = $(this).data("nome");
                $(".btn-danger").data("idcontato", $(this).data("id"));
                $("#contatosModal .modal-body .relacao").hide(500, function () {
                    $(".mnu-btn").addClass("hidden");
                    $(".del-contato p:first span").text(nome);
                    $(".del-contato").removeClass("hidden");
                });
            });
        },
        error: function (data) {
            $("#msgErroContato span").text(data.statusText);
            $("#msgErroContato").hide().removeClass('hide').fadeIn(500);
            $("div.modal-header span").remove();
            setTimeout(function () { $("#msgErroContato").fadeOut(500); }, 3000);
        },
    });
};

$(".del-contato p .btn-danger").on("click", function (e) {
    e.preventDefault();
    ExcluirContato($(this).data("idcontato"));
});
$(".del-contato p .vorta").on("click", function (e) {
    e.preventDefault();
    $(".del-contato").addClass("hidden");
    $("#contatosModal .modal-body .relacao").show(500, function () {
        $(".mnu-btn").removeClass("hidden");
    });
});

var ExcluirContato = function (id) {
    var strUrl = "/Cadastro/DeleteContato";
    $.ajax(
        {
            type: 'POST',
            url: strUrl,
            dataType: 'text',
            cache: false,
            async: true,
            data: { id: id },
            beforeSend: function () {
                $(".del-contato p .btn-danger span").text("");
                $(".del-contato p .btn-danger i").addClass("glyphicon glyphicon-refresh icon-refresh-animate");
            },
            success: function (data) {
                if (data.length == 0) {
                    var loading = "<span><em>Buscando dados no servidor... </em>&nbsp;<i class='glyphicon glyphicon-refresh icon-refresh-animate'></i></span>";
                    $('#contatosModal .modal-header h4').after(loading);
                    CarregaContatos();

                } else {
                    $("#msgErroContato span").text(data);
                    $("#msgErroContato").hide().removeClass('hide').fadeIn(500);
                    setTimeout(function () { $("#msgErroContato").fadeOut(500); }, 3000);
                }
            },
            error: function (data) {
                $("#msgErroContato span").text(data.statusText);
                $("#msgErroContato").hide().removeClass('hide').fadeIn(500);
                setTimeout(function () { $("#msgErroContato").fadeOut(500); }, 3000);
            },
            complete: function () {
                $(".del-contato").addClass("hidden");
                $(".del-contato p .btn-danger i").removeClass("glyphicon glyphicon-refresh icon-refresh-animate");
                $(".del-contato p .btn-danger span").text("Sim");
                $("#contatosModal .modal-body .relacao").show(500);
            },
        });
};

$(".mnu-btn button").on('click', function (e) {
    e.preventDefault();
    var acao = $(this).data("acao");
    if (acao == "add") {
        $("#contatosModal .modal-body .relacao").hide(500, function () {
            $(".dados-contato").removeClass("hidden");
            $("#nomeContato").focus();
        });
        $(this).data("acao", "del");
        $(this).children().removeClass("glyphicon-plus").addClass("glyphicon-remove");
    } else {
        $(".dados-contato").addClass("hidden");
        $("#contatosModal .modal-body .relacao").show(500);
        $(this).data("acao", "add");
        $(this).children().removeClass("glyphicon-remove").addClass("glyphicon-plus");
    }
});

$("#salvarContato").on("click", function (e) {
    e.preventDefault();
    var nome = $("#nomeContato");
    // ... por favor pule essa parte, vá direto para o ajax
    if (nome.val().trim().length == 0) {
        //:(
        nome.focus().parent().addClass("has-error");
    } else {
        var empresa = $("#contatosModal .modal-body input[type=hidden]").val();
        var strUrl = "/Cadastro/SaveContato";
        $.ajax(
        {
            type: 'POST',
            url: strUrl,
            dataType: 'text',
            cache: false,
            async: true,
            data: {
                EmpresaID: empresa, Nome: nome.val(),
                Telefone: $("#telefoneContato").val(), Celular: $("#celularContato").val(),
                Email: $("#emailContato").val()
            },
            beforeSend: function () {
                $("#salvarContato i").removeClass("glyphicon-ok").addClass("glyphicon-refresh icon-refresh-animate");
            },
            success: function (data) {
                if (data.length == 0) {
                    $(".dados-contato").addClass("hidden");
                    $(".mnu-btn button").data("acao", "add");
                    $(".mnu-btn button").children().removeClass("glyphicon-remove").addClass("glyphicon-plus");
                    var loading = "<span><em>Buscando dados no servidor... </em>&nbsp;<i class='glyphicon glyphicon-refresh icon-refresh-animate'></i></span>";
                    $('#contatosModal .modal-header h4').after(loading);
                    CarregaContatos();
                    $('.dados-contato input').val('');
                } else {
                    //$("#msgErroContato span").text(data);
                    $("#msgErroContato span").text("Erro no cadastro. Verifique a validação dos campos!");
                    nome.focus();
                    $("#msgErroContato").hide().removeClass('hide').fadeIn(500);
                    setTimeout(function () { $("#msgErroContato").fadeOut(500); }, 5000);
                }
            },
            error: function (data) {
                $("#msgErroContato span").text(data.statusText);
                $("#msgErroContato").hide().removeClass('hide').fadeIn(500);
                setTimeout(function () { $("#msgErroContato").fadeOut(500); }, 3000);
            },
            complete: function () {
                $("#salvarContato i").removeClass("glyphicon-refresh icon-refresh-animate").addClass("glyphicon-ok");

            },
        });
    }
});

$("#nomeContato").on("keyup", function () {
    if ($(this).val().length > 0) {
        $("#nomeContato").parent().removeClass("has-error");
    }
});
