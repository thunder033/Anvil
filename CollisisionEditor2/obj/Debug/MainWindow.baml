   M S B A M L   `   `   `  ����@  <Anvil, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:CollisisionEditor2CollisisionEditor2  5      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5      #    CollisisionEditor2.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   2local clr-namespace:CollisisionEditor2     5      9System&clr-namespace:System;assembly=mscorlib     5      -   5          �Name$  Animation_Manager��5   	    � Title$ ThunderForge Anvil��5   	   $	��720��6$   $
��1280��61    � WindowState$ 	Maximized=�6>    � Icon$ Media/anvil-icon2-med.png��6V    � DataContext 5   	   ��  �� RelativeSource �� *Self+ � 	Resources 5	       �� MergedDictionaries �� Source	 �� 
TargetType 
  Header
 �� Property �� Value2   ,CollisisionEditor2.PolygonSelectionConverter  SelectionConverter  PolygonTypeValues S� 
MethodName S� 
ObjectType  System.Enum S� MethodParameters M� TypeName�� 5
   
    5      �� 5      $! Resources/DesignerItem.xaml?�6%   %�  &	        &	 �     &	     �� 5      "	 ��6   .�5      �� 
 � ��$
 Bold"�6/   �� 5      
 � �� �	#FF5B5B5B6/   �� 5      
 � �� �#EEEEEE6/   �� 5      
 � ��$ 1,0,0,0q�64   �� 5      
 � �� �#99999960   5        5      6%   S� 5      $ 	GetValues��62   "  6I    5      M� 5      $ local:PolygonType��6   5!      5"      .��5%      �  � RowDefinitions 5&   
   �� 5'      $��50 �6   �� 5(      $��* �6    � ColumnDefinitions 5*   
   �� 5+      $	��250 �6   �� 5,      $��* �6   �� 5-      $	��225 �6   .+�50   
   �� $��3��6   ���#555555$��0,0,0,1q�6;   ���#F3F3F36U   �� 52   
   $��1q�6   
���   �$��1��69   $��1��6I   $��2��6V   ._�53      �� -   5         ��Name$ TextureWrapper��53   2   $��1��6   $��1��6%    J� RenderTransformOrigin$ 0.5,0.5&�6J   
������� 54        MoveThumbTemplate �� 
TargetType"   CollisisionEditor2.MoveThumb  ResizeDecoratorTemplate$   CollisisionEditor2.ResizeThumb � Cursor  DesignerItemTemplate��%�  &	       &	 B     &	 �    7  7  �� 55      "  6@   .R�56      �� 
������ 6$   57      �� 59      " ��6F   .R�5:      � .+�5;        $��3��60   $ SizeNS��6;   $��0 -4 0 0q�6K   $	��Top=�5<      $��Stretch=�6(     5=      $��3��60   $ SizeWE��6:   $��-4 0 0 0q�6J   $��Stretch=�5>      $
��Left=�6,     5?      $��3��60   $ SizeWE��6:   $��0 0 -4 0q�6J   $��Stretch=�5@      $��Right=�6,     5A      $��3��60   $ SizeNS��6;   $��0 0 0 -4q�6K   $��Bottom=�5B      $��Stretch=�6,     5C      $��7��60   $��7��6:   $ SizeNWSE��6E   $��	-6 -6 0 0q�6W   $	��Top=�5D      $
��Left=�6(     5E      $��7��60   $��7��6:   $ SizeNESW��6E   $��	0 -6 -6 0q�6W   $	��Top=�5F      $��Right=�6(     5G      $��7��60   $��7��6:   $ SizeNESW��6E   $��	-6 0 0 -6q�6W   $��Bottom=�5H      $
��Left=�6+     5I      $��7��60   $��7��6:   $ SizeNWSE��6E   $��	0 0 -6 -6q�6W   $��Bottom=�5J      $��Right=�6+   5K      5L      �� 5N      " ��6C   .R�5O      �  6   ��  �� *TemplatedParent+.+�5P        8��  $ SizeAll��6\   �� 5Q      8�� 6&   �� 5R      #��z��6/   5S      5T      5U      z�5V      V� 5W      .~�5X      �� �� 5Y       �� AngleX$ 0.249Q�6(   �� 5Z      T� 5[       T� X$ 1.496Q�6-   5\      .+�5^      �� -   5         ��Name$ TextureBorder��5^      $
��Left=�60   $	��Top=�6K   
�������$��1q�6v   ._�5_      �� -   5         ��Name$ Texture��5_      $
��Left=�6-   $	��264��6H   $	��Top=�6U   $	��497��6m   5`      5a      5b      �� 5d   
   -   5         ��Name$ openSettings��5d      $��24��6'   $��24��62   $
��Left=�6>   $��	10,10,0,0q�6Y   $	��Top=�6l   $��3q�6�   .��5e      �� $��Media/gear.png��6   $
��Fill=�6,   5f      �� 5h   
   $��0��6   $��1��6   $��0, 10q�6-   $��50��6<   $
��Left=�6G   $��Frame��6b   �� 5i   
   -   5        $ 	BackFrame��5i      $��<<��6$   $��44��67   $��29��6B   $��1��6N   $��0��6^   $
��Left=�6k   $��
50,10,0,10q�6�   6�   �� 5j   
   -   5         ��Name$ CurrentFrame��5j       �� TextWrapping$
 Wrap=�6(   $��0��6<   $��100,10,0,10q�6E   $��44��6Z   $
��Left=�6e   $��Center=�6�   $��Center=�6�   $��29��6�   $��0��6�   $��1��6�   �� 5k   
   -   5        $ AdvanceFrame��5k      $��>>��6'   $��44��6:   $��29��6E   $��0��6Q   $��1��6^   $
��Left=�6n   $��150,10,0,10q�6�   6�   �� 5l   
   -	   5        $ ToolbarBtn1��5l      $��Add Rectangle��6&   $��0��6T   $��1��6a   $	��120��6q   $
��Left=�6}   $��215,10,10,10q�6�   �� 5m   
   -
   5        $ 
RectDelete��5m      $��Delete Rectangle��6%   $��1��6@   $��0��6P   $
��Left=�6]   $��345,10,10,10q�6x   $	��120��6�   
���  ��6�   �� 5o   
   -   5        $	��New��5o      $��75��6   $��Right=�6*   $	��Top=�6F   $��1��6^   $��3q�6k    K� ZIndex$ 5��6v   $��2��6�   �� 5p   
   $��
Animations��6   $��1��6%   $��2��62   $��0q�6B   $��2��6M   $	��Top=�6^   #��[  6v   �� 5q      -   5         ��Name$ AnimationList��5q      $��1��6#   $��2��60   $��0,26,0,0q�6@   $	��300��6R   $	��Top=�6_   $ 2��6w   $��2��6�   $��Stretch=�6�   $��0,0,0,1q�6�   ���#444444��5r      �� 5s      .R�5t      �� -   5        $��Stretch=�5t   L   $��Stretch=�6q   $��Stretch=�6�   ��6   �� *.+6�   5u      5w      �� 5y      $��Animation Properties��6
   $��0��6)   $��1��69   $	��Top=�6F   $��0q�6^   $ 5��6i   $��2��6z   #��[  6�   �� 5z   
   -   5        $ 
EditButton��5z       $
��Edit��6   $	��Top=�64   $��Right=�6L   $��1��6i   $��65��6v   $��3q�6�   $ 6��6�   6�   �� 5{   
   $��0��6   $��1��6!   $	��Top=�6.   $��0,26,0,0q�6F   $	��300��6X   $��2��6e   $ 2��6v   
���   �$��0q�6�   ._�5|      �� -   5         ��Name$ AnimationInfo��5|   $   $��1��6   $	��300��6;   $ 3��6H   $
��Left=�6Y     �� View  5}      �� 5~      .P�5      �� ! �� Width$	! 115��6)   " �� DisplayMemberBinding" 65   �� # �� Path$# Property��� 5�      $	! 105��6)   " 65   �� $# Value�5�      5�      5�      �� 5�   
   $��Collision Polygon��6   $��0��6,   $��1��6<   $	��Top=�6I   $��	0,327,0,0q�6a   $ 5��6t   $��2��6�   #��[  6�   �� 5�   
   -   5        $ CollisionPolygonList��5�      $��0��60   $��1��6@   $	��Top=�6M   $��	0,353,0,0q�6e   $ 5��6x   $��2��6�   $	��200��6�   $��Stretch=�6�   ��5�      �� 5�      .R�5�      �  5�      �� 5�   	   $��2* �6   �� 5�   	   $��* �6   .+�5�      �� $��2��6]   ��6   �� *
selected+$ �� 	Converter#$ [ 6]   �� 5�      -   5        $��0��5�   N   $
��Left=�6^   ��6   �� *
dimensions+6^   �� 5�      $��1��6'   ��6   �� *type+6'   �� 5�      -   5        $��83��5�   �   $��1��6�   $ 2��6�   % �� SelectedItem% 6   �� *type+& �� Mode$& TwoWay=���6;   �� ' �� Source#' [ y�6}   �� *
selected+#$ [ 6Q  5�      5�      5�      �� 5�   
   -   5        $ customFieldsOpen��5�      $��Edit Custom Fields��6+   $��1��6H   $	��Top=�6U   $��	0,555,0,0q�6m   $��5q�6�   6�   5�      5�      