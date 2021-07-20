

////////////////////////////////////////////////////////////////////EVENTO EXPANDIR-CONTRAER/////////////////////////////////////
//Colapsar Ventana
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
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function soloNumeros(e) {
    var key;
    if (window.event) // IE
    {
        key = e.keyCode;
    }
    else if (e.which) // Netscape/Firefox/Opera
    {
        key = e.which;
    }

    if (key < 48 || key > 57) {
        return false;
    }
    return true;
}


function convMayusculas(control) {
    var str = $("[id*=" + control + "]").val();
    var strMayus = str.toUpperCase();
    $("[id*=" + control + "]").val(strMayus);
}



function soloNumeros(e) {
    var key;
    if (window.event) // IE
    {
        key = e.keyCode;
    }
    else if (e.which) // Netscape/Firefox/Opera
    {
        key = e.which;
    }

    if (key < 48 || key > 57) {
        return false;
    }
    return true;
}


function charRFC(e) {
    var key;
    if (window.event) // IE
    {
        key = e.keyCode;
    }
    else if (e.which) // Netscape/Firefox/Opera
    {
        key = e.which;
    }

    if (key > 47 && key < 58) {
        return true;
    }

    if (key > 64 && key < 91) {
        return true;
    }

    if (key > 96 && key < 123) {
        return true;
    }

    return false;
}



function validarFecha(control) {
    var dato = $("[id*=" + control + "]").val();
    var fecha = dato.trim();

    try {
        var fecha = fecha.split("/");
        var dia = fecha[0];
        var mes = fecha[1];
        var ano = fecha[2];
        var estado = true;

        if ((dia.length == 2) && (mes.length == 2) && (ano.length == 4)) {
            switch (parseInt(mes)) {
                case 1: dmax = 31; break;
                case 2: if (ano % 4 == 0) dmax = 29; else dmax = 28; break;
                case 3: dmax = 31; break;
                case 4: dmax = 30; break;
                case 5: dmax = 31; break;
                case 6: dmax = 30; break;
                case 7: dmax = 31; break;
                case 8: dmax = 31; break;
                case 9: dmax = 30; break;
                case 10: dmax = 31; break;
                case 11: dmax = 30; break;
                case 12: dmax = 31; break;
            }

            dmax != "" ? dmax : dmax = -1;

            if ((dia >= 1) && (dia <= dmax) && (mes >= 1) && (mes <= 12)) {
                for (var i = 0; i < fecha[0].length; i++) {
                    diaC = fecha[0].charAt(i).charCodeAt(0);
                    (!((diaC > 47) && (diaC < 58))) ? estado = false : '';
                    mesC = fecha[1].charAt(i).charCodeAt(0);
                    (!((mesC > 47) && (mesC < 58))) ? estado = false : '';
                }

            }

            for (var i = 0; i < fecha[2].length; i++) {
                anoC = fecha[2].charAt(i).charCodeAt(0);
                (!((anoC > 47) && (anoC < 58))) ? estado = false : '';
            }
        }
        else estado = false;
        return estado;

    } catch (err) {
        alert("Error fechas");
    }
}



function obtenerEdad() {
    let hoy = new Date();
    var fechaNac = $("[id*=txt_fecNac]").val();
    var fechaNacFto = fechaNac.substring(3, 5) + "/" + fechaNac.substring(0, 2) + "/" + fechaNac.substring(6, 10);
    let fechaNacimiento = new Date(fechaNacFto);
    let edad = hoy.getFullYear() - fechaNacimiento.getFullYear();
    let diferenciaMeses = hoy.getMonth() - fechaNacimiento.getMonth();
    if (diferenciaMeses < 0 || (diferenciaMeses === 0 && hoy.getDate() < fechaNacimiento.getDate())) {
        edad--;
    }
    $("[id*=txt_edad]").val(edad)
}




function telefono(e, control) {
    var key;
    if (window.event) // IE
    {
        key = e.keyCode;
    }
    else if (e.which) // Netscape/Firefox/Opera
    {
        key = e.which;
    }

    if (key < 48 || key > 57) {
        return false;
    }


    if (LengthCheck(control)) {
        return true;
    }
    else {
        return false;
    }
}


function LengthCheck(control) {
    var dato = $("[id*=" + control + "]").val();
    var long = dato.length;

    if (long > 9) {
        //fn_MuestraMensaje('Motivo Rechazo', "Ha alcanzado el numero de caracteres permitidos", 0);
        //alert("Caracteres permitidos");
        return false;
    }
    return true;
}


function formatoMoneda(control) {
    var monto = $("[id*=" + control + "]").val();

    var coma = true

    while (coma) {
        if (monto.indexOf(',') !== -1) {
            monto = monto.replace(",", "");
            coma = true
        }
        else {
            coma = false
        }
    }

    monto += '';
    var arrMonto = monto.split('.');
    var entero = arrMonto[0];
    var decimal = arrMonto.length > 1 ? '.' + arrMonto[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(entero)) {
        entero = entero.replace(rgx, '$1' + ',' + '$2');
    }

    var montoFinal = entero + decimal;
    $("[id*=" + control + "]").val(montoFinal);
    //return entero + decimal;
}

function importe(e) {
    var key;
    if (window.event) // IE
    {
        key = e.keyCode;
    }
    else if (e.which) // Netscape/Firefox/Opera
    {
        key = e.which;
    }

    if (key == 46) {
        return true;
    }

    if (key < 48 || key > 57) {
        return false;
    }
    return true;
}



$("body").on('click', '[id*=chkFisica]', function (e) {

    if ($(this).is(':checked')) {
        $("[id*=chkMoral]").prop('checked', false);
        $("[id*=txt_apMat]").prop('disabled', false);
        $("[id*=txt_nombres]").prop('disabled', false);
    }
    else {
        $("[id*=chkFisica]").prop('checked', true);
    }
});


$("body").on('click', '[id*=chkMoral]', function (e) {

    if ($(this).is(':checked')) {
        $("[id*=chkFisica]").prop('checked', false);
        $("[id*=txt_apMat]").prop('disabled', true);
        $("[id*=txt_nombres]").prop('disabled', true);
    }
    else {
        $("[id*=chkMoral]").prop('checked', true);
    }
});

//VZAVALETA_10290_CC_INICIO
$("body").on('focusout', '[id*=txt_codTercero]', function (e) {
    CargarBeneficiario(e);
});

function CargarBeneficiario(e) {

    //if ($("[id*=txt_codTercero]").val() == '' && $("[id*=Catalogo]").is(":visible") == false) 
        
        var beneficiario = $("input[id$='txt_codTercero']")[0].value;
        if (beneficiario.trim().length > 0) {

            $('#EsperaModal').modal('toggle');
            fn_CargaCatalogo("BenTercero_stro", beneficiario.trim(), "", "Unica", "txt_codTercero|txt_codTercero|txt_codTercero|txt_codTercero", "Terceros", "block");                   
            }

         ////else {
        //    e.stopPropagation();
        
    }
//VZAVALETA_10290_CC_FIN