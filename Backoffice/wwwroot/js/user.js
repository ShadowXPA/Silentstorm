$(() => {
    const baseUrl = $('#base-url').val()
    const csrfToken = $('input[name="__RequestVerificationToken"]').val();

    $('.user').each((i, u) => {
        const user = $(u)
        const userId = user.attr('data-id')
        const deleteBtn = user.find('.delete')

        deleteBtn.off('click')
            .on('click', e => {
                e.stopPropagation()
                $('#delete-user .btn-danger').off('click')
                    .on('click', e => {
                        $.ajax(`${baseUrl}user/${userId}`, {
                            method: 'DELETE',
                            data: {
                                __RequestVerificationToken: csrfToken
                            },
                            beforeSend: (xhr, settings) => {
                                Spinner.add()
                            },
                            success: (data, status, xhr) => {
                                window.location.href = `${baseUrl}user`
                            },
                            error: (xhr, status, error) => {
                                alert(`An error occured trying to delete the user... ${error}`)
                            },
                            complete: (xhr, status) => {
                                $('#delete-user').modal('hide')
                                Spinner.remove()
                            }
                        })
                    })
                $('#delete-user').modal('show')
            })
    })
})
