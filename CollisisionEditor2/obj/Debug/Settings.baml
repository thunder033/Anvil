   M S B A M L   `   `   `  ����@  <Anvil, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:CollisisionEditor2CollisisionEditor2  5      '    !CollisisionEditor2.SettingsWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   2local clr-namespace:CollisisionEditor2     5      -   5          �Name$  Settings��5   	    � Title$ Settings��5   	   $	��450��6   $	��650��6'    � Icon$ Media/anvil-icon2-med.png��63    � 
ResizeMode$ NoResize=�6T    � DataContext 5      ��  �� RelativeSource �� *Self+ � 	Resources 5         projectDirectory �� 
TargetType �� Property	 �� Value��%X   &	        �� 5      " ��6#   .�5	      ��  � ��	 �#8888886"   5
      5      .��5      � 
 � ColumnDefinitions
 5      �� 5      $	��210 �6   �� 5      $��* �6   .+�5      �� $��Project Directory��6
   $
��Left=�6&   $��	20,20,0,0q�6A   $	��Top=�6T   �� 5   
   -   5         ��Name$ projectDirectory��5      $��1��6,   $	��370��6<   $
��Left=�6H   $	��Top=�6c   $��0,20,0,0q�6{   $��5q�6�   �� 5      -   5         ��Name$ projectDirectoryBrowseBtn��5      $	��...��6.   $��1��6<   $
��Left=�6L   $	��Top=�6g   $��30��6   $��5q�6�   $��
370,20,0,0q�6�   ���	#FFEEEEEE���	#FF5B5B5B6�   �� 5      $��Textures Directory��6
   $
��Left=�6'   $��	20,57,0,0q�6B   $	��Top=�6U   �� 5      $�� {Project Directory}\��6
   $��1��6*   $
��Left=�6:   $	��Top=�6U   $��0,58,0,0q�6m   #��[  6   �� 5      -   5        $ texturesDirectory��5      $��1��6'   $	��400��67   $
��Left=�6C   $	��Top=�6^   $��0,57,0,0q�6v   $��	107,5,5,5q�6�   
������ 6�   �� 5      $��Textures Index File��6
   $
��Left=�6(   $��	20,94,0,0q�6C   $	��Top=�6V   �� 5      $0��* {Project Directory}\{Textures Directory}\��6
   $��1��6?   $
��Left=�6O   $	��Top=�6j   $��0,95,0,0q�6�   #��[  6�   �� 5      -   5        $ texturesIndex��5      $��1��6#   $	��400��63   $
��Left=�6?   $	��Top=�6Z   $��0,94,0,0q�6r   $��	216,5,5,5q�6�   
������ 6�   �� 5      $%��Animation Definitions Directory��6
   $
��Left=�64   $��
20,143,0,0q�6O   $	��Top=�6c   �� 5      -   5        $ animationDefsDir��5      $��1��6&   $	��400��66   $
��Left=�6B   $	��Top=�6]   $��	0,143,0,0q�6u   $��	107,5,5,5q�6�   �� 5      $�� {Project Directory}\��6
   $��1��6*   $
��Left=�6:   $	��Top=�6U   $��	0,144,0,0q�6m   #��[  6�   �� 5!      $ ��Animation Defintions Index��6
   $
��Left=�6/   $��
20,180,0,0q�6J   $	��Top=�6^   �� 5"      $2��, {Project Directory}\{Anim. Def. Directory}\��6
   $��1��6A   $
��Left=�6Q   $	��Top=�6l   $��	0,181,0,0q�6�   #��[  6�   �� 5#      -   5        $ animationIndex��5#      $��1��6$   $	��400��64   $
��Left=�6@   $	��Top=�6[   $��	0,180,0,0q�6s   $��	225,5,5,5q�6�   
������ 6�   �� 5&      $��Animation File Type��6
   $
��Left=�6(   $��
20,217,0,0q�6C   $	��Top=�6W   �� 5'      -   5        $ animationFileExt��5'      $��1��6&   $	��400��66   $
��Left=�6B   $	��Top=�6]   $��	0,217,0,0q�6u   $��5q�6�   �� 5)      $��Texture Preview Directory��6
   $
��Left=�6.   $��
20,263,0,0q�6I   $	��Top=�6]   �� 5*      $�� {Project Directory}\��6
   $��1��6*   $
��Left=�6:   $	��Top=�6U   $��	0,264,0,0q�6m   #��[  6�   �� 5+      -	   5        $ animationSheetDir��5+      $��1��6'   $	��400��67   $
��Left=�6C   $	��Top=�6^   $��	0,263,0,0q�6v   $��	107,5,5,5q�6�   
������ 6�   �� 5-   
   $��Animation Fields��6   $
��Left=�6+   $��
20,315,0,0q�6F   $	��Top=�6Z   �� 5.      -
   5        $ customFieldsCSV��5.      $��1��6%   $	��400��65   $
��Left=�6A   $	��Top=�6\   $��	0,315,0,0q�6t   $��5q�6�   
������ 6�   �� 50      -   5        $ 
saveButton��50      $
��Save��6   $��1��6.   $��Right=�6>   $��Bottom=�6Z   $��5q�6u   $��80��6�   $��20q�6�   6�   51      52      