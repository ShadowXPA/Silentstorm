$(() => {
    $('#project').off('change')
        .on('change', e => {
            const project = $(e.currentTarget)
            const projectId = project.val()
            $('#submission option').not(`[data-project-id="${projectId}"]`).addClass('d-none')
            $(`#submission option[data-project-id=${projectId}]`).removeClass('d-none')
            $('#submission > option[value=""]').removeClass('d-none')
        })
})
