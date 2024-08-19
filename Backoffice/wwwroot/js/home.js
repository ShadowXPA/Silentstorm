$(() => {
    const baseUrl = $('#base-url').val()

    $('.project').off('click')
        .on('click', (e) => {
            const projectElement = $(e.currentTarget)
            window.location.href = `${baseUrl}project/${projectElement.attr('data-id')}`
        })
})
