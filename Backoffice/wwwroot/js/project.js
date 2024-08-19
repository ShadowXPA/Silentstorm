$(() => {
    const baseUrl = $('#base-url').val()
    const projectId = $('#project-id').val()
    const csrfToken = $('input[name="__RequestVerificationToken"]').val();

    $('.project-actions #delete').off('click')
        .on('click', e => {
            $('#delete-project').modal('show')
        })

    $('#delete-project .btn-danger').off('click')
        .on('click', e => {
            $.ajax(`${baseUrl}project/${projectId}`, {
                method: 'DELETE',
                data: {
                    __RequestVerificationToken: csrfToken
                },
                beforeSend: (xhr, settings) => {
                    Spinner.add()
                },
                success: (data, status, xhr) => {
                    window.location.href = baseUrl
                },
                error: (xhr, status, error) => {
                    alert(`An error occured trying to delete the project... ${error}`)
                },
                complete: (xhr, status) => {
                    Spinner.remove()
                }
            })
        })

    $('.project-actions #advance').off('click')
        .on('click', e => {
            $('#advance-project').modal('show')
        })

    $('#advance-project .btn-success').off('click')
        .on('click', e => {
            $.ajax(`${baseUrl}project/${projectId}/advance`, {
                method: 'POST',
                data: {
                    __RequestVerificationToken: csrfToken
                },
                beforeSend: (xhr, settings) => {
                    Spinner.add()
                },
                success: (data, status, xhr) => {
                    window.location.href = `${baseUrl}project/${projectId}`
                },
                error: (xhr, status, error) => {
                    alert(`An error occured trying to update the project... ${error}`)
                },
                complete: (xhr, status) => {
                    Spinner.remove()
                }
            })
        })

    $('.announcement').each((i, a) => {
        const announcement = $(a)
        const projectStatus = announcement.attr('data-project-status')
        const resetBtn = announcement.find('.reset')
        const deleteBtn = announcement.find('.delete')

        resetBtn.off('click')
            .on('click', e => {
                e.stopPropagation()
                $('#reset-announcement .btn-primary').off('click')
                    .on('click', e => {
                        $.ajax(`${baseUrl}announcement/reset`, {
                            method: 'POST',
                            data: {
                                __RequestVerificationToken: csrfToken,
                                projectId,
                                projectStatus
                            },
                            beforeSend: (xhr, settings) => {
                                Spinner.add()
                            },
                            success: (data, status, xhr) => {
                                window.location.href = `${baseUrl}project/${projectId}`
                            },
                            error: (xhr, status, error) => {
                                alert(`An error occured trying to update the project... ${error}`)
                            },
                            complete: (xhr, status) => {
                                $('#reset-announcement').modal('hide')
                                Spinner.remove()
                            }
                        })
                    })
                $('#reset-announcement').modal('show')
            })

        deleteBtn.off('click')
            .on('click', e => {
                e.stopPropagation()
                $('#delete-announcement .btn-danger').off('click')
                    .on('click', e => {
                        $.ajax(`${baseUrl}announcement`, {
                            method: 'DELETE',
                            data: {
                                __RequestVerificationToken: csrfToken,
                                projectId,
                                projectStatus
                            },
                            beforeSend: (xhr, settings) => {
                                Spinner.add()
                            },
                            success: (data, status, xhr) => {
                                window.location.href = `${baseUrl}project/${projectId}`
                            },
                            error: (xhr, status, error) => {
                                alert(`An error occured trying to update the project... ${error}`)
                            },
                            complete: (xhr, status) => {
                                $('#delete-announcement').modal('hide')
                                Spinner.remove()
                            }
                        })
                    })
                $('#delete-announcement').modal('show')
            })
    })

    $('.submission').each((i, s) => {
        const submission = $(s)
        const submissionId = submission.attr('data-id')
        const isSelectedForVoting = submission.attr('data-is-selected-for-voting')
        const deleteBtn = submission.find('.delete')
        const songLink = submission.find('a')

        if (isSelectedForVoting) {
            submission.off('click')
                .on('click', e => {
                    $('.votes').not(`[data-submission-id="${submissionId}"]`).addClass('d-none')
                    $(`.votes[data-submission-id="${submissionId}"]`).toggleClass('d-none')
                })
        }

        songLink.off('click')
            .on('click', e => {
                e.preventDefault()
                e.stopPropagation()
                window.open(e.currentTarget.href, '_blank', 'noopener')
            })

        deleteBtn.off('click')
            .on('click', e => {
                e.stopPropagation()
                $('#delete-submission .btn-danger').off('click')
                    .on('click', e => {
                        $.ajax(`${baseUrl}submission`, {
                            method: 'DELETE',
                            data: {
                                __RequestVerificationToken: csrfToken,
                                'project.id': projectId,
                                id: submissionId
                            },
                            beforeSend: (xhr, settings) => {
                                Spinner.add()
                            },
                            success: (data, status, xhr) => {
                                window.location.href = `${baseUrl}project/${projectId}`
                            },
                            error: (xhr, status, error) => {
                                alert(`An error occured trying to update the project... ${error}`)
                            },
                            complete: (xhr, status) => {
                                $('#delete-submission').modal('hide')
                                Spinner.remove()
                            }
                        })
                    })
                $('#delete-submission').modal('show')
            })
    })

    $('.vote').each((i, v) => {
        const vote = $(v)
        const userId = vote.attr('data-user-id')
        const deleteBtn = vote.find('.delete')

        deleteBtn.off('click')
            .on('click', e => {
                e.stopPropagation()
                $('#delete-vote .btn-danger').off('click')
                    .on('click', e => {
                        $.ajax(`${baseUrl}vote`, {
                            method: 'DELETE',
                            data: {
                                __RequestVerificationToken: csrfToken,
                                'project.id': projectId,
                                'user.id': userId
                            },
                            beforeSend: (xhr, settings) => {
                                Spinner.add()
                            },
                            success: (data, status, xhr) => {
                                window.location.href = `${baseUrl}project/${projectId}`
                            },
                            error: (xhr, status, error) => {
                                alert(`An error occured trying to update the project... ${error}`)
                            },
                            complete: (xhr, status) => {
                                $('#delete-vote').modal('hide')
                                Spinner.remove()
                            }
                        })
                    })
                $('#delete-vote').modal('show')
            })
    })
})
