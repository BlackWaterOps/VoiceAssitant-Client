package com.stremor.plexi.interfaces;

import com.stremor.plexi.models.Choice;
import com.stremor.plexi.models.ShowModel;

/**
 * Classes implementing this interface are expected to be the bridge between the user and the Plexi
 * backend. These classes provide implementations for interacting with the user and retrieving user
 * input.
 */
public interface IPlexiListener {
    /**
     * Show a response to the user.
     *
     * @param show An object describing the content to be shown.
     * @param speak A natural-language string which can be spoken to the user.
     */
    public void show(ShowModel show, String speak);

    /**
     * Present a choice of values to the user. When the user has completed a selection, the client
     * should call
     * {@link com.stremor.plexi.interfaces.IPlexiService#choice(com.stremor.plexi.models.Choice)}
     * with the chosen string result.
     *
     * @param choices
     */
    public void requestChoice(Choice[] choices);

    /**
     * Handle an error detected by the Plexi backend.
     *
     * @param message A natural-language message describing the error.
     */
    public void error(String message);

    /**
     * Handle an internal error experienced by the Plexi backend.
     */
    public void internalError();
}
