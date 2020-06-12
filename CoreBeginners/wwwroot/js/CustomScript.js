function ConfirmDelete(id, status) {
    var deletespan = 'DeleteSpan_' + id;
    var confirmdeletespan = 'ConfirmationDeleteUser_' + id;
    if (status) {
        $('#' + confirmdeletespan).show();
        $('#' + deletespan).hide();
    } else {
        $('#' + confirmdeletespan).hide();
        $('#' + deletespan).show();
    }
}