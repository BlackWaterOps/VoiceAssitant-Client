package com.stremor.plexi.client;

import android.R;
import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.TextView;

import com.stremor.plexi.client.models.ConversationItem;

import java.util.List;

public class ConversationAdapter extends ArrayAdapter<ConversationItem> {
    private LayoutInflater mInflater;

    public ConversationAdapter(Context context, int textViewResourceId, List<ConversationItem> objects) {
        super(context, textViewResourceId, objects);

        mInflater = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        View rowView = mInflater.inflate(R.layout.simple_list_item_1, null);
        ((TextView) rowView.findViewById(R.id.text1)).setText(getItem(position).getText());

        return rowView;
    }
}
