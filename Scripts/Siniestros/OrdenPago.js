﻿$(document).ready(function () {
    $("#contenedor_principal").addClass("table OP_stro");
    $("#lbl_Titulo").text("SINIESTROS");
});

$("body").on("click", ".contraer", function () {
    event.preventDefault ? event.preventDefault() : event.returnValue = false;
    var id = this.id.substr(this.id.length - 1)
    fn_CambiaEstado(id, "1");
});

//Expandir Ventana
$("body").on("click", ".expandir", function () {
    event.preventDefault ? event.preventDefault() : event.returnValue = false;
    var id = this.id.substr(this.id.length - 1)
    fn_CambiaEstado(id, "0");
});

$("body").on("change", "[id*=cmbTipoUsuario]", function () {

    $("[id*=txtCodigoBeneficiario_stro]").text('');
    $("[id*=txtCodigoBeneficiario_stro]").val('');
    //$("[id*=txtBeneficiario_stro]").text('');
    //$("[id*=txtBeneficiario_stro]").val('');
    $("input[id$='txtBeneficiario_stro']")[0].value = "";

    switch ($(this).val()) {
        case "10":
            $("[id*=Onbase]").show();
            $("[id*=pnlProveedor]").show();
            break;
        case "7":
        case "8":
            $("[id*=Onbase]").hide();
            $("[id*=pnlProveedor]").hide();
            break;
    }

});

$("body").on('focusout', '[id*=txtBeneficiario_stro]', function (e) {

    if ($("[id*=cmbTipoUsuario]").val() == "7" || $("[id*=cmbTipoUsuario]").val() == "10") { //Asegurado o Proveedor
        e.preventDefault ? e.preventDefault() : e.returnValue = false;
    }
    else {
        CargarBeneficiario(e);
    }

});

$("body").on('focusout', '[id*=txtRFC]', function (e) {

    if ($("[id*=cmbTipoUsuario]").val() == "7" || $("[id*=cmbTipoUsuario]").val() == "10") { //Asegurado o Proveedor
        e.preventDefault ? e.preventDefault() : e.returnValue = false;
    }
    else {
        CargarBeneficiarioRFC(e);
    }

});

$("body").on('keydown', '[id*=txtBeneficiario_stro]', function (e) {

    if ($("[id*=cmbTipoUsuario]").val() == "7" || $("[id*=cmbTipoUsuario]").val() == "10") { //Asegurado o Proveedor
        e.preventDefault ? e.preventDefault() : e.returnValue = false;
    }
    else {
        $("[id*=txtCodigoBeneficiario_stro]").text('');
        $("[id*=txtCodigoBeneficiario_stro]").val('');
        //$("input[id$='txtBeneficiario_stro']")[0].value = "hola";
    }

    var keyCode = e.keyCode || e.which;

    switch (keyCode) {
        case 9:
            var beneficiario = $("input[id$='txtBeneficiario_stro']")[0].value;

            if (beneficiario.trim().length > 0) {

                $('#EsperaModal').modal('toggle');

                e.preventDefault ? e.preventDefault() : e.returnValue = false;

                switch ($("[id*=cmbTipoUsuario]").val()) {
                    case "7": //Asegurado
                        fn_CargaCatalogo("Ben_stro", beneficiario.trim(), "", "Unica", "txtCodigoBeneficiario_stro|txtBeneficiario_stro", "Asegurados");
                        break;
                    case "8": //Tercero
                        fn_CargaCatalogo("BenTercero_stro", beneficiario.trim(), "", "Unica", "txtCodigoBeneficiario_stro|txtBeneficiario_stro", "Terceros");
                        break;
                    case "10": //Proveedor
                        fn_CargaCatalogo("BenProveedor_stro", beneficiario.trim(), "", "Unica", "txtCodigoBeneficiario_stro|txtBeneficiario_stro", "Proveedores");
                        break;
                    default: console.log("error");
                }

            }
            else {
                alert("Ingrese un nombre o razón social");
                e.stopPropagation();
            }
            break;

        case 13:
            e.preventDefault();
            return false;
            break;
        default:
    }

});

$("body").on('keydown', '[id*=txtSiniestro]', function (e) {

    $("[id*=txtPoliza]").val('');
    $("[id*=txtMonedaPoliza]").val('');
    $("[id*=txtNombre]").val('');
    $("[id*=txtCodigoBeneficiario_stro]").val('');
    $("[id*=txtBeneficiario]").val('');
    $("[id*=txtRFC]").val('');
    $("[id*=cmbSubsiniestro]").empty();
    $("[id*=txtNumeroComprobante]").val('');
    $("[id*=txtFechaComprobante]").val('');

    var keyCode = e.keyCode || e.which;

    if (keyCode == 13) {
        e.preventDefault();
        return false;
    }
});

$("body").on('paste', '[id*=txtSiniestro]', function (e) {
    $("[id*=txtPoliza]").val('');
    $("[id*=txtMonedaPoliza]").val('');
    $("[id*=txtNombre]").val('');
    $("[id*=txtCodigoBeneficiario_stro]").val('');
    $("[id*=txtBeneficiario]").val('');
    $("[id*=txtRFC]").val('');
    $("[id*=cmbSubsiniestro]").empty();
    $("[id*=txtNumeroComprobante]").val('');
    $("[id*=txtFechaComprobante]").val('');
});

$("body").on('click', '[id*=chkVariasFacturas]', function (e) {

    if ($(this).is(':checked')) {
        $("#Comprobantes").hide();
    }
    else {
        $("#Comprobantes").show();
    }

});

$("body").on('change', '[id*=cmbTipoPagoOP]', function (e) {

    switch ($(this).val()) {

        case "C":
            break;
        case "T":
            break;

    }

});

function ValidarBeneficiario() {

    if ($("[id*=txtCodigoBeneficiario_stro]").val() == '' || $("[id*=txtBeneficiario_stro]").val().trim() == '') {
        fn_MuestraMensaje("OrdenPagoSiniestros", "Nombre o razón social no definido.", 0);
        return false;
    }
    return true;

}

function fn_ImprimirOrden(Server, strOrden) {
    var nro_op = strOrden.split(",");
    for (i = 0 ; i < nro_op.length; i++) {
        window.open(Server.replace('@nro_op', nro_op[i]));
    }
}
function CargarBeneficiarioRFC(e) {

    if ($("[id*=txtCodigoBeneficiario_stro]").val() == '' && $("[id*=Catalogo]").is(":visible") == false) {

        var beneficiario = $("input[id$='txtRFC']")[0].value;
        if (beneficiario.trim().length > 0) {

            $('#EsperaModal').modal('toggle');

            switch ($("[id*=cmbTipoUsuario]").val()) {
                case "8": //Tercero
                    fn_CargaCatalogo("BenTerceroRFC_stro", beneficiario.trim(), "", "Unica", "txtCodigoBeneficiario_stro|txtBeneficiario_stro|txtRFC", "Terceros", "block");
                    break;
                case "10": //Proveedor
                    fn_CargaCatalogo("BenProveedor_stro", beneficiario.trim(), "", "Unica", "txtCodigoBeneficiario_stro|txtBeneficiario_stro", "Proveedores");
                    break;
            }

        }
        else {
            e.stopPropagation();
        }

    }

}
function CargarBeneficiario(e) {

    if ($("[id*=txtCodigoBeneficiario_stro]").val() == '' && $("[id*=Catalogo]").is(":visible") == false) {

        var beneficiario = $("input[id$='txtBeneficiario_stro']")[0].value;
        if (beneficiario.trim().length > 0) {

            $('#EsperaModal').modal('toggle');

            switch ($("[id*=cmbTipoUsuario]").val()) {
                case "8": //Tercero
                    fn_CargaCatalogo("BenTercero_stro", beneficiario.trim(), "", "Unica", "txtCodigoBeneficiario_stro|txtBeneficiario_stro|txtRFC", "Terceros","block");
                    break;
                case "10": //Proveedor
                    fn_CargaCatalogo("BenProveedor_stro", beneficiario.trim(), "", "Unica", "txtCodigoBeneficiario_stro|txtBeneficiario_stro", "Proveedores");
                    break;
            }

        }
        else {
            e.stopPropagation();
        }

    }

}



