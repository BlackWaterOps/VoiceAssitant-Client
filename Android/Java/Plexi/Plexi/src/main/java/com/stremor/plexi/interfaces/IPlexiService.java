package com.stremor.plexi.interfaces;

import com.stremor.plexi.models.Choice;

/**
 * Created by jeffschifano on 10/28/13.
 */
public interface IPlexiService {

    public String getOriginalQuery();

    void clearContext();

    void resetTimer();

    void query(String query);

    void choice(Choice choice);
}
