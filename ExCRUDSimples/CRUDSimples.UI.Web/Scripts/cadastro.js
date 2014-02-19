/*
    by FabianoNalin.net.br
*/

$(".view-contatos").on('click', function (e) {
    e.preventDefault();
    var elemento = this;
    $("#rel-contatos").slideToggle("fast", function () {
        if ($(this).data("flag") == "0") {
            $(elemento).removeClass("glyphicon glyphicon-chevron-down").addClass("glyphicon glyphicon-chevron-up");
            $(this).data("flag", "1");
        } else {
            $(elemento).removeClass("glyphicon glyphicon-chevron-up").addClass("glyphicon glyphicon-chevron-down");
            $(this).data("flag", "0");
        }
    });
});

$(".close-contato").on("click", function (e) {
    var elemento = $(this);
    $(elemento).parent().hide(500, function () {
        $(elemento).parent().remove();
    });
});

$(".edit-contato").on("click", function (e) {
    var elemento = $(this);
    var id = $(elemento).data("id");
    $("#contatoModalLabel").text("Editar Contato");
    $("#contato-id").val(id);
    $("#view-ID").val(id);
    var loading = "<span><em>Buscando dados no servidor... </em>&nbsp;<i class='glyphicon glyphicon-refresh icon-refresh-animate'></i></span>";
    $('#contatosModal .modal-header h4').after(loading);
    $('#contatosModal').modal('show');
});

$("#btn-add-contato").on("click", function (e) {
    e.preventDefault();
    $("#contatoModalLabel").text("Adicionar Contato");
    $("#contato-id").val('novo');
    $("#view-ID").val('Auto');
    $('#contatosModal').modal('show');
});


//executado qdo modal acionado e antes de exibí-lo
$('#contatosModal').on('show.bs.modal', function (e) {
    $("#contato-nome").val('');
    $("#contato-telefone").val('');
    $("#contato-email").val('');
    $("#contato-celular").val('');

});

//executado qdo modal acionado e após de exibí-lo
$('#contatosModal').on('shown.bs.modal', function (e) {
    var id = $("#contato-id").val();
    if (isNaN(id)) {
        $("#contato-nome").val('');
    } else {
        CarregaContato(id);
    }
    $("#contato-nome").focus();
});

var CarregaContato = function (id) {
    var strUrl = "/Cadastro/GetContato/" + id;
    $.ajax(
    {
        type: 'GET',
        url: strUrl,
        dataType: 'json',
        cache: false,
        async: true,
        success: function (data) {
            $("div.modal-header span").remove();
            if (data.erro.length == 0) {
                $("#contato-nome").val(data.dados.nome);
                $("#contato-telefone").val(data.dados.telefone);
                $("#contato-email").val(data.dados.email);
                $("#contato-celular").val(data.dados.celular);
            } else {
                ExibeErro(data.erro);
            }
        },
        error: function (data) {
            ExibeErro(data.statusText);
        },
    });
};

var ExibeErro = function (msg) {
    $("#msgErroContato span").text(msg);
    $("#msgErroContato").hide().removeClass('hide').fadeIn(500);
    $("div.modal-header span").remove();
    setTimeout(function () { $("#msgErroContato").fadeOut(500); }, 3000);
};

$(".modal-footer button.btn.btn-danger").on('click', function () {
    var nome = $("#contato-nome").val();
    if (nome.length == 0) {
        $("#contato-nome").focus().parent().addClass("has-error");
    } else {
        var tel = $("#contato-telefone").val();
        var email = $("#contato-email").val();
        var cel = $("#contato-celular").val();

        /* ****** 
            Add ou Edit na listagem de contatos
            Action já tá preparada para receber os contatos no post
            :(
                Não fiz pois já são 3h da madruga e vou dormir, mas se for contratado prometo fazer! (:
        */
    }
});

$("#contato-nome").on("keyup", function () {
    var elemento = this;
    if ($(elemento).val().length > 0) {
        $(elemento).parent().removeClass("has-error");
    }
});
